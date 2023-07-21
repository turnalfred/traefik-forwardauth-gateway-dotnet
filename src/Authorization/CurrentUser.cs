using System.Security.Claims;

namespace ForwardAuthGateway.Authorization;

public class CurrentUser
{
    public ClaimsPrincipal Principal { get; set; } = default!;
}