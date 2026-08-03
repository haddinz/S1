using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.DTOs.Request;

public sealed record UserProfileRequest
{
    [Required(ErrorMessage = "FullName Required")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "UserName Required")]
    public string UserName { get; init; } = string.Empty;
    
    [Required(ErrorMessage = "PhoneNumber Required")]
    [Phone(ErrorMessage = "Invalid Phone Number Format")]
    public string? PhoneNumber { get; init; }
}