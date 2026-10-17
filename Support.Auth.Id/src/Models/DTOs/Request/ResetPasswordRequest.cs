using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record ResetPasswordRequest
{
    [Required]
    public string Token { get; init; } = string.Empty;

    [Required(ErrorMessage = "New Password Required")]
    public string NewPassword { get; init; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword),ErrorMessage = "Password confirmation does not match.")]
    public string ConfirmNewPassword { get; init; } = string.Empty;
}