using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Repositories.Interfaces;

public interface IRoleRepositories : IBaseRepositories
{
    Task<Role> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Role> GetRoleByTypeAsync(string roleType, CancellationToken cancellationToken = default);
    Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    Task<bool> IsRoleTypeExists(string roleType, CancellationToken cancellationToken = default);
    Task<bool> IsRoleExists(Guid id, CancellationToken cancellationToken = default);
    
    Task AddRoleAsync(Role role, CancellationToken cancellationToken);
    Task UpdateRoleAsync(Role role, CancellationToken cancellationToken);


    // ToDo : For GET scenario will get update using Dapper instead using EFcore for coplectifitas query
    Task<(IReadOnlyCollection<Role> Roles, int TotalPages)> GetPagedRolesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}