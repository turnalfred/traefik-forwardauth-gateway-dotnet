using System.Security.Claims;
using ForwardAuthGateway.Authentication;
using ForwardAuthGateway.Authorization;
using ForwardAuthGateway.Exceptions;

namespace ForwardAuthGateway.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/oauth2/user")
            .WithTags("oauth2")
            .RequireAuthorization();

        group.MapGet("/",
            (HttpContext ctx, IClaimMappingService mappingService) =>
            {
                if (ctx.User.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.SidClaimType) is not
                    { } sessionId)
                {
                    throw new MissingClaimException(ClaimTypes.Sid);
                }

                var provider = ctx.User.Claims.FirstOrDefault(x =>
                    x.Type == AuthenticationConstants.InternalChallengeProviderClaimType);

                return Task.FromResult(TypedResults.Ok(new UserInfoResponse
                {
                    UserEndpoints = new UserInfoResponse.Endpoints
                    {
                        Logout = $"/oauth2/signout/{provider?.Value}?sid={sessionId.Value}"
                    },
                    UserInfo = mappingService.MapCurrentUserClaimsToUserInfo()
                }));
            })
            .WithDescription("Get the current user's info configured from the claim mappings.");

        return group;
    }
}

public class UserInfoResponse
{
    public required IDictionary<string, string> UserInfo { get; set; }
    public required Endpoints UserEndpoints { get; set; }

    public class Endpoints
    {
        public required string Logout { get; set; }
    }
}