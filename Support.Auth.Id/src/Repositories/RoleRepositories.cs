using Microsoft.EntityFrameworkCore;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repository.Data;

namespace Support.Auth.Id.Repositories;

public class RoleRepositories : BaseRepositories, IRoleRepositories
{
    public RoleRepositories(AppDbContext dbContext)
        : base(dbContext) { }

    public async Task AddRoleAsync(Role role, CancellationToken cancellationToken)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
    }

    public async Task<Role> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return role!;
    }

    public async Task<Role> GetRoleByTypeAsync(string roleType, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleType == roleType, cancellationToken);

        return role!;
    }

    public async Task<Role> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);

        return role!;
    }

    public async Task<bool> IsRoleTypeExists(string roleType, CancellationToken cancellationToken = default)
    {
        bool roleIdExists = await _context.Roles.AnyAsync(x => x.RoleType == roleType, cancellationToken);

        return roleIdExists;
    }

    public async Task<bool> IsRoleExists(Guid id, CancellationToken cancellationToken = default)
    {
        bool roleIdExists = await _context.Roles.AnyAsync(x => x.Id == id, cancellationToken);

        return roleIdExists;
    }

    public Task UpdateRoleAsync(Role role, CancellationToken cancellationToken)
    {
        _context.Roles.Update(role);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyCollection<Role> Roles, int TotalPages)> GetPagedRolesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        IQueryable<Role> queryRole = _context.Roles.AsNoTracking();
        int totalRecords = await queryRole.CountAsync(cancellationToken);

        List<Role> roles = await queryRole
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return(roles, totalRecords);
    }

}