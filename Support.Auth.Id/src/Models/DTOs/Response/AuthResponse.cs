namespace Support.Auth.Id.Models.DTOs.Response;

public sealed record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}
