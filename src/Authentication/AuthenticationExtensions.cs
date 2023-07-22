using System.Security.Claims;
using ForwardAuthGateway.Exceptions;
using ForwardAuthGateway.Extensions;
using ForwardAuthGateway.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Net.Http.Headers;

namespace ForwardAuthGateway.Authentication;

public static class AuthenticationExtensions
{
    private static string _forwardAuthDefaultScheme = nameof(_forwardAuthDefaultScheme);


    public static string GetAuthenticationSchemeNameFromProviderName(string name)
    {
        return $"ForwardAuthChallengeScheme__{name.ToLowerInvariant()}";
    }

    public static void AddForwardAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var openIdOptions =
            services.AddConfigurationFromSectionName<ForwardAuthOptions>(configuration,
                ForwardAuthOptions.SectionName);

        if (openIdOptions.TokenManagement.Enabled)
        {
            services.AddOpenIdConnectAccessTokenManagement(x =>
            {
                // TODO can we support multiple challenge schemes?
                x.ChallengeScheme = GetAuthenticationSchemeNameFromProviderName(openIdOptions.OptionalDefaultProvider);
                x.RefreshBeforeExpiration = openIdOptions.TokenManagement.RefreshBeforeExpiry;
            });
        
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = openIdOptions.TokenManagement.RedisConnectionString;
                options.InstanceName = openIdOptions.TokenManagement.RedisInstanceName;
            });
        }

        var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = _forwardAuthDefaultScheme;

                if (string.IsNullOrEmpty(openIdOptions.OptionalDefaultProvider)) return;

                options.DefaultChallengeScheme =
                    GetAuthenticationSchemeNameFromProviderName(openIdOptions.OptionalDefaultProvider);

                options.DefaultSignOutScheme =
                    GetAuthenticationSchemeNameFromProviderName(openIdOptions.OptionalDefaultProvider);
            })
            .AddCookie(_forwardAuthDefaultScheme, (options) =>
            {
                options.ExpireTimeSpan = openIdOptions.CookieOptions.ExpiryTimeSpan;
                options.SlidingExpiration = openIdOptions.CookieOptions.SlidingExpiration;
                options.Cookie.Name = openIdOptions.CookieOptions.CookieName;
                options.Cookie.SameSite = openIdOptions.CookieOptions.SameSiteMode;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                
                if (openIdOptions.TokenManagement.Enabled)
                {
                    // TODO L1/L2 cache implementation
                    options.SessionStore = new RedisCacheTicketStore(new RedisCacheOptions
                    {
                        Configuration = openIdOptions.TokenManagement.RedisConnectionString
                    });
                }
                
                options.Events.OnSigningIn = context =>
                {
                    if (context.Principal is null)
                    {
                        throw new ArgumentNullException(nameof(context.Principal));
                    }

                    // user-designated claim checks
                    foreach (var claimCheck in openIdOptions.CookieOptions.RequiredClaimValueChecks ?? Array.Empty<RequiredClaimValueCheck>())
                    {
                        if (!context.Principal.HasClaim(x =>
                                string.Equals(x.Type, claimCheck.ClaimType,
                                    StringComparison.InvariantCultureIgnoreCase) && string.Equals(x.Value,
                                    claimCheck.ClaimValue.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            throw new UnauthorizedException(
                                $"User {context.Principal.Identity?.Name} was expected to have claim '{claimCheck.ClaimType}':'{claimCheck.ClaimValue}'.");
                        }
                    }

                    return Task.CompletedTask;
                };

                options.Events.OnSigningOut = async e =>
                {
                    await e.HttpContext.RevokeRefreshTokenAsync();
                };
            });

        foreach (var provider in openIdOptions.Providers)
        {
            var schemeName = GetAuthenticationSchemeNameFromProviderName(provider.Name);
            authBuilder.AddOpenIdConnect(schemeName, options =>
            {
                // general options
                options.SignInScheme = _forwardAuthDefaultScheme;
                options.Authority = provider.OpenIdOptions.Authority.ToString();
                options.ClientId = provider.OpenIdOptions.ClientId;
                options.ClientSecret = provider.OpenIdOptions.ClientSecret;
                options.ResponseType = provider.OpenIdOptions.ResponseType;
                options.UsePkce = provider.OpenIdOptions.UsePkce;

                // session cookie options
                options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                // sign-out
                options.SignOutScheme = schemeName;
                options.SignedOutRedirectUri = "/p/welcome";
                options.RemoteSignOutPath = $"/oauth2/signout/{provider.Name}";
                options.SignedOutCallbackPath = $"/oauth2/signout-oidc/{provider.Name}";
                options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    await context.HttpContext.SignOutAsync(_forwardAuthDefaultScheme);
                };
                
                // scopes
                options.Scope.Clear();
                foreach (var scope in provider.OpenIdOptions.Scopes)
                {
                    options.Scope.Add(scope);
                }

                // claim mapping options
                options.MapInboundClaims = false;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = openIdOptions.TokenManagement.Enabled;

                // add the IDP  the user authenticated with for later use
                options.ClaimActions.Add(new AddStaticClaimAction(AuthenticationConstants.InternalChallengeProviderClaimType,
                    ClaimValueTypes.String, provider.Name));

                // run user-specified claim mappings for this provider
                foreach (var claimTransformation in provider.ClaimTransformationOptions?.Transformations ??
                                                    Array.Empty<ClaimTransformation>())
                {
                    claimTransformation.AddClaimActions(options.ClaimActions);
                }

                // sign-in
                options.CallbackPath = $"/oauth2/signin-oidc/{provider.Name}";

                const string interactivePathPrefix = "/oauth2/check/interactive";
                var providerLoginPathPrefix = $"/oauth2/login/{provider.Name}";
                var allowedRedirectPathPrefixes = new[] { interactivePathPrefix, providerLoginPathPrefix };

                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    if (!allowedRedirectPathPrefixes.Any(x =>
                            context.Request.Path.ToString().StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        // force 401 response for any request other than the interactive checks
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }

                    if (!string.IsNullOrEmpty(openIdOptions.OptionalInteractiveChallengeRedirectOverrideUrl) &&
                        context.Request.Path.ToString().StartsWith(interactivePathPrefix))
                    {
                        context.Response.StatusCode = StatusCodes.Status307TemporaryRedirect;
                        context.Response.Headers.Add(HeaderNames.Location,
                            openIdOptions.OptionalInteractiveChallengeRedirectOverrideUrl);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }

                    // Note - the redirectURIs below will return post-login (its not the OIDC redirectURI)
                    if (!string.IsNullOrEmpty(provider.OptionalBaseReturnUrl))
                    {
                        // return to a specified return path post-login
                        context.Properties.RedirectUri = new UriBuilder(provider.OptionalBaseReturnUrl)
                        {
                            Path = context.Properties.Parameters[AuthenticationConstants.ReturnUrlPathParameterName]?.ToString()
                        }.Uri.ToString();
                    }
                    else
                    {
                        // return to the requested Uri post-login
                        var host = context.Request.Headers.Host;
                        var scheme = context.Request.Headers[TraefikForwardAuthHeaders.Protocol];
                        var path = context.Request.Headers[TraefikForwardAuthHeaders.RequestUri];
                        context.Properties.RedirectUri = $"{scheme}://{host}{path}";
                    }


                    if (!Uri.IsWellFormedUriString(context.Properties.RedirectUri, UriKind.Absolute))
                    {
                        throw new UndeterminedReturnUrlException(context.Properties.RedirectUri);
                    }

                    // force redirect URI to https
                    context.ProtocolMessage.RedirectUri = new UriBuilder(context.ProtocolMessage.RedirectUri)
                    {
                        Scheme = Uri.UriSchemeHttps,
                    }.Uri.ToString();

                    return Task.CompletedTask;
                };
            });
        }
    }
}