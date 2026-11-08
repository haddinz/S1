using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request.Role;

public sealed record UpdateRoleRequest
{
    [Required]
    [MaxLength(50)]
    public string RoleType { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RoleName { get; init; } = string.Empty;

    [MaxLength(100)]
    public string Description { get; init; } = string.Empty;
}
