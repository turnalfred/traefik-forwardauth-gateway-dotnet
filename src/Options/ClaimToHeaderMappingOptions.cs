namespace ForwardAuthGateway.Options;

public class ClaimToHeaderMappingOptions
{
    public Dictionary<string, string> Mappings { get; set; } = new();
    public MissingClaimAction MissingClaimAction { get; set; } = MissingClaimAction.Throw;
}