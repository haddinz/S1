using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.Entity;

public class Register
{
    [Required(ErrorMessage = "Name is Required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is Required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is Required")]
    [StringLength(
        40,
        MinimumLength = 8,
        ErrorMessage = "The {0} must be at {2} and at max {1} character long"
    )]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm Password is Required")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Password Does Not Match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
