using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.Entity;

public class VerifiEmail
{
    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
