using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request.Role;

public sealed record CreateRoleRequest
{
    [Required]
    [MaxLength(50)]
    public string RoleType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Description { get; set; } = string.Empty;
}