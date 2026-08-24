using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.DTOs;

public sealed class ResendVerifyEmailRequest
{
    [Required(ErrorMessage = "Email resend verify required")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
}