using System.ComponentModel.DataAnnotations;

namespace ForwardAuthServer.Api.Options;

public class OpenIdOptions
{
    [Required]
    public Uri Authority { get; set; } = null!;
    
    [Required]
    public string ResponseType { get; set; } = string.Empty;
    
    [Required]
    public string[] Scopes { get; set; } = Array.Empty<string>();
    
    [Required]
    public string ClientId { get; set; } = string.Empty;
    
    [Required]
    public string ClientSecret { get; set; } = string.Empty;
    
    [Required]
    public bool UsePkce { get; set; }
}