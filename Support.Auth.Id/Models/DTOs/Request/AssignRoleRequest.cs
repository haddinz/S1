using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Id.Models.Dtos.Request;

public class AssignRoleRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}
