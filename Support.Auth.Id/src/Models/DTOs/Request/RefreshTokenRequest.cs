using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh Token Required")]
    public string RefreshToken { get; init; } = string.Empty;
}
