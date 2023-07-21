namespace ForwardAuthGateway.Options;

public class RequiredClaimValueCheck
{
    public string ClaimType { get; set; } = null!;
    public object ClaimValue { get; set; } = null!;
}