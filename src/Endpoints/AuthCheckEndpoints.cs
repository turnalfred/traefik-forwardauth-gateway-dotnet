using ForwardAuthGateway.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    static readonly Delegate CheckHandler = (HttpContext ctx, [FromServices] IClaimMappingService mappingService) =>
    {
        foreach (var (header, value) in mappingService.MapCurrentUserClaimsToHeaders())
        {
            ctx.Response.Headers.Add(header, value);
        }

        return TypedResults.Ok();
    };
}