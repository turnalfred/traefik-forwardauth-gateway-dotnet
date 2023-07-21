using System.ComponentModel.DataAnnotations;

namespace ForwardAuthServer.Api.Options;

public class ProviderOptions
{
    [Required]
    private string _name = default!;
    
    public string Name
    {
        get => _name;
        set => _name = value.ToLowerInvariant();
    }
    
    [Required]
    public string DisplayName { get; set; } = default!;
    
    [Required]
    public OpenIdOptions OpenIdOptions { get; set; } = default!;
    
    [Required]
    public ClaimTransformationOptions ClaimTransformationOptions { get; set; } = default!;
    
    public string OptionalBaseReturnUrl { get; set; } = string.Empty;
}