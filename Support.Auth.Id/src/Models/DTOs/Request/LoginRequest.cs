using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.DTOs.Request;

public sealed record LoginRequest
{
    [Required(ErrorMessage = "Email Required")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password Required")]
    public string Password { get; init; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; init; }
}
