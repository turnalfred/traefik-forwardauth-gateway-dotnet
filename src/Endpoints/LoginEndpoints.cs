using ForwardAuthGateway.Authentication;
using ForwardAuthGateway.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForwardAuthGateway.Endpoints;

public static class LoginEndpoints
{
    public static RouteGroupBuilder MapLoginEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/oauth2")
            .WithTags("oauth2");

        group.MapGet("/providers",
                ([FromServices] IOptions<ForwardAuthOptions> options) =>
                    Task.FromResult(TypedResults.Ok(options.Value.Providers.Select(x => new ProviderInfoResponseItem
                    {
                        Name = x.DisplayName,
                        LoginUrl = $"/oauth2/login/{x.Name}"
                    }))))
            .AllowAnonymous()
            .WithDescription("Get a list of active IDPs supported by the AuthServer.");

        group.MapGet("/login/{provider}", ([FromRoute] string provider,
                [FromQuery] string? returnPath,
                IOptions<ForwardAuthOptions> options) =>
            {
                if (!options.Value.Providers.Any(x =>
                        string.Equals(x.Name, provider, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return Task.FromResult(Results.NotFound());
                }

                var properties = new AuthenticationProperties();
                if (!string.IsNullOrEmpty(returnPath) && Uri.IsWellFormedUriString(returnPath, UriKind.Relative))
                {
                    properties.Parameters.Add(AuthenticationConstants.ReturnUrlPathParameterName, returnPath);
                }

                return Task.FromResult(Results.Challenge(properties,
                    new[] { AuthenticationExtensions.GetAuthenticationSchemeNameFromProviderName(provider) }));
            }).AllowAnonymous()
            .WithDescription("Trigger an interactive redirect to the IDP.");

        return group;
    }
}

public class ProviderInfoResponseItem
{
    public required string Name { get; set; }
    public required string LoginUrl { get; set; }
}