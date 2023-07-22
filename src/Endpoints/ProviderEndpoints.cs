using ForwardAuthGateway.Authentication;
using ForwardAuthGateway.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ForwardAuthGateway.Endpoints;

public static class ProviderEndpoints
{
    public static RouteGroupBuilder MapProviderEndpoints(this IEndpointRouteBuilder routes)
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

        return group;
    }
}

public class ProviderInfoResponseItem
{
    public required string Name { get; set; }
    public required string LoginUrl { get; set; }
}