using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record VerifyEmailRequest
{
    [Required(ErrorMessage = "Verify Email Token Required")]
    public string Token { get; init; } = string.Empty;
}