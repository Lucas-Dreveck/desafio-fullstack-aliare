namespace Aliare.Weather.Api.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationInMinutes { get; set; } = 15;
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
