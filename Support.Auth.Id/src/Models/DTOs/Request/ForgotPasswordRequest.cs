using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.DTOs.Request;

public sealed record ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email Is Required")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}