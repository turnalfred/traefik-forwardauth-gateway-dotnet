using System.ComponentModel.DataAnnotations;

namespace ForwardAuthGateway.Options;

public class TokenManagementOptions
{
    [Required]
    public bool Enabled { get; set; }
    
    [Required]
    public TimeSpan RefreshBeforeExpiry { get; set; }
    
    [Required]
    public string RedisConnectionString { get; set; } = string.Empty;
    
    [Required]
    public string RedisInstanceName { get; set; } = string.Empty;
    
    [Required]
    public string AccessTokenForwardHeaderKey { get; set; } = string.Empty;
}