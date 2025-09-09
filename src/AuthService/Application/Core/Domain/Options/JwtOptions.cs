using System.ComponentModel.DataAnnotations;

namespace Domain.Options;

public sealed class JwtOptions
{
    [Required]
    public string ValidIssuer { get; init; } = string.Empty;
    
    [Required]
    public string ValidAudience { get; init; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int AccessTokenExpires { get; init; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int RefreshTokenExpires { get; init; }
}