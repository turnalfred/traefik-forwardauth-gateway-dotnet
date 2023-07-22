using Duende.AccessTokenManagement.OpenIdConnect;
using ForwardAuthGateway.Authorization;
using ForwardAuthGateway.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForwardAuthGateway.Endpoints;

public static class AuthCheckEndpoints
{
    public static RouteGroupBuilder MapAuthCheckEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/oauth2/check")
            .WithTags("oauth2")
            .RequireAuthorization();

        group.MapGet("/", CheckHandler)
            .WithDescription(
                "Check if the user is logged in. Will return a 401 if not. ForwardHeaders will be returned based on the header:claim mappings.");

        group.MapGet("/interactive", CheckHandler)
            .WithDescription(
                "Check if the user is logged in. Will trigger an interactive redirect if not. ForwardHeaders will be returned based on the header:claim mappings.");

        return group;
    }

    private static readonly Delegate CheckHandler = async (HttpContext ctx,
        [FromServices] IClaimMappingService mappingService,
        [FromServices] IOptions<ForwardAuthOptions> options,
        [FromServices] IUserTokenManagementService userTokenManagementService) =>
    {
        if (options.Value.TokenManagement.Enabled)
        {
            var token = await userTokenManagementService.GetAccessTokenAsync(ctx.User);
            ctx.Response.Headers.Add(options.Value.TokenManagement.AccessTokenForwardHeaderKey, token.AccessToken);
        }
        
        foreach (var (header, value) in mappingService.MapCurrentUserClaimsToHeaders())
        {
            ctx.Response.Headers.Add(header, value);
        }

        return TypedResults.Ok();
    };
}