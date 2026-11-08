using System.ComponentModel.DataAnnotations;

namespace Support.Auth.Client.DTOs.Request;

public sealed record RevokeRoleRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid RoleId { get; init; }
}
