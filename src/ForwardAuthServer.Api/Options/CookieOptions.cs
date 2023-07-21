using System.ComponentModel.DataAnnotations;

namespace ForwardAuthServer.Api.Options;

public class CookieOptions
{
    [Required]
    public string CookieName { get; set; } = string.Empty;
    
    [Required]
    public SameSiteMode SameSiteMode { get; set; }
    
    [Required]
    public TimeSpan ExpiryTimeSpan { get; set; }
    
    [Required]
    public bool SlidingExpiration { get; set; }
    
    [Required]
    public RequiredClaimValueCheck[] RequiredClaimValueChecks { get; set; } = null!;
}