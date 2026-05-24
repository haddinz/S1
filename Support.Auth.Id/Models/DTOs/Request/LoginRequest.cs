using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.Entity;

public class LoginRequest
{
    [Required(ErrorMessage = "Email Required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
