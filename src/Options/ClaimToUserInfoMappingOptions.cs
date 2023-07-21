namespace ForwardAuthGateway.Options;

public class ClaimToUserInfoMappingOptions
{
    public Dictionary<string, string> Mappings { get; set; } = new();
    public MissingClaimAction MissingClaimAction { get; set; } = MissingClaimAction.Throw;
}