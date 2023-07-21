namespace ForwardAuthGateway.Options;

public struct TraefikForwardAuthHeaders
{
    public const string Method = "X-Forwarded-Method";
    public const string Protocol = "X-Forwarded-Proto";
    public const string Host = "X-Forwarded-Host";
    public const string RequestUri = "X-Forwarded-Uri";
    public const string SourceIpAddress = "X-Forwarded-For";
}