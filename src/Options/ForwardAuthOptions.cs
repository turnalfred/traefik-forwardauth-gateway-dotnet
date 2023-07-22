using System.ComponentModel.DataAnnotations;

namespace ForwardAuthGateway.Options;

public class ForwardAuthOptions
{
    public const string SectionName = "ForwardAuth";

    [Required]
    public CookieOptions CookieOptions { get; set; } = default!;

    [Required]
    public ICollection<ProviderOptions> Providers { get; set; } = default!;

    [Required]
    public ClaimToHeaderMappingOptions ClaimToHeaderMappings { get; set; } = default!;

    [Required]
    public ClaimToUserInfoMappingOptions ClaimToUserInfoMappings { get; set; } = default!;
    
    [Required]
    public TokenManagementOptions TokenManagement { get; set; } = default!;

    public string OptionalInteractiveChallengeRedirectOverrideUrl { get; set; } = string.Empty;

    public string OptionalDefaultProvider { get; set; } = string.Empty;
}