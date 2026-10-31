using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request.Role;

public sealed record CreateRoleRequest
{
    [Required]
    public string RoleType { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}