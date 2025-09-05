namespace Domain.Options;

public class JwtOptions
{
    public string ValidIssuer { get; init; } = string.Empty;
    public string ValidAudience { get; init; } = string.Empty;
    public int Expires { get; init; }
}