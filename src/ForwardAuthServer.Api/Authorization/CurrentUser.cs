using System.Security.Claims;

namespace ForwardAuthServer.Api.Authorization;

public class CurrentUser
{
    public ClaimsPrincipal Principal { get; set; } = default!;
}