namespace Support.Auth.Id.Services.Security;

public sealed class JwtSettings
{
    // Using init for imutable field after startup for configuration
    public const string SectionName = "JwtSettings";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SecurityKey { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; }
    public int RefreshTokenExpirationDays { get; init; }
}
