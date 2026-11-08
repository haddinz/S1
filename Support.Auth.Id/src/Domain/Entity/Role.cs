using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;

namespace Support.Auth.Id.Models.Entity;

public class Role : BaseModels
{
    public string RoleType { get; private set; } = string.Empty;
    public string RoleName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Many to many = relasion one role can have many users
    public ICollection<User> Users { get; private set; } = new List<User>();

    private Role()
        : base() { }

    public static Role Create(string roleType, string roleName, string desc)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            throw new ArgumentNullException(nameof(roleName));
        }

        if (string.IsNullOrEmpty(roleType))
        {
            throw new ArgumentNullException(nameof(roleType));
        }

        return new Role
        {
            RoleName = roleName,
            RoleType = roleType,
            Description = desc,
        };
    }

    public void Update(string roleType, string roleName, string description)
    {
        if (string.IsNullOrWhiteSpace(roleType))
            throw new BusinessValidationException("Role type cannot be empty.");

        if (string.IsNullOrWhiteSpace(roleName))
            throw new BusinessValidationException("Role name cannot be empty.");

        RoleType = roleType.Trim();
        RoleName = roleName.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

        SetUpdate();
    }
}
