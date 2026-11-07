namespace Support.Auth.Client.DTOs.Response;

public sealed record RoleResponse
{
    public Guid Id { get; init; }
    public string RoleType { get; init; } = string.Empty;
    public string RoleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
