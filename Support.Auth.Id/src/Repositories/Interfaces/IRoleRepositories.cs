using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Repositories.Interfaces;

public interface IRoleRepositories : IBaseRepositories
{
    // Query path: read-only, detached entity (no tracking).
    Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Role> Roles, int TotalRecords)> GetPagedRolesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );

    // Command path: tracked aggregate used for state changes.
    Task<Role?> GetTrackedRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsRoleInUseAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByTypeAsync(string roleType, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    Task<bool> IsRoleTypeExists(string roleType, CancellationToken cancellationToken = default);
    Task<bool> IsRoleNameExists(string roleName, CancellationToken cancellationToken = default);
    Task<bool> IsRoleExists(Guid id, CancellationToken cancellationToken = default);

    Task AddRoleAsync(Role role, CancellationToken cancellationToken);
}
