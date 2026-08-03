using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record ChangePasswordRequest()
{
    [Required(ErrorMessage = "Password Required")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "NewPassword Required")]
    public string NewPassword { get; init; } = string.Empty;
}