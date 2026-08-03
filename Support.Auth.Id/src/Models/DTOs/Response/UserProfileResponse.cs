namespace Support.Auth.Id.Models.DTOs.Response;

public sealed record UserProfileResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public bool IsEmailVerified { get; init; }
    public bool IsActive { get; init; }
}