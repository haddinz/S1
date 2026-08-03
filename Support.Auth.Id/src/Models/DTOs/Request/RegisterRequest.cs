using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record RegisterRequest
{
    [Required(ErrorMessage = "Email Required")]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "FullName Required")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "UserName Required")]
    public string UserName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Password Required")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = string.Empty;

    // [Required(ErrorMessage = "PhoneNumber Required")]
    // [Phone(ErrorMessage = "Invalid Phone Number Format")]
    public string PhoneNumber { get; init; } = string.Empty;
}
