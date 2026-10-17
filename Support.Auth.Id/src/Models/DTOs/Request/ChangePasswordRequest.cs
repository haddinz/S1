using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record ChangePasswordRequest()
{
    [Required(ErrorMessage = "Current Password Required")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "NewPassword Required")]
    public string NewPassword { get; init; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword),ErrorMessage = "Password confirmation does not match.")]
    public string ConfirmNewPassword { get; init; } = string.Empty;

    
}