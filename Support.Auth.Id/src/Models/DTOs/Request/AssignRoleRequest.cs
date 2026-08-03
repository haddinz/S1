using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record AssignRoleRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}
