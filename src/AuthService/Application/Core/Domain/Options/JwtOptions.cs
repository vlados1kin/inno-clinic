namespace Domain.Options;

public record JwtOptions(string SecretKey, string ValidIssuer, string ValidAudience, int Expires);