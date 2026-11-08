using System.Data.Common;
using Dapper;
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

    public async Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT "Id", "RoleType", "RoleName", "Description", "CreatedAt", "UpdateAt", "IsDeleted"
            FROM "Roles"
            WHERE "Id" = @Id AND "IsDeleted" = false
            LIMIT 1;
            """;

        DbConnection connection = _context.Database.GetDbConnection();

        Role? role = await connection.QuerySingleOrDefaultAsync<Role>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken)
        );

        return role;
    }

    public async Task<Role?> GetTrackedRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return role;
    }

    public async Task<bool> IsRoleInUseAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        bool roleInUse = await _context.Users.AnyAsync(
            user => user.Roles.Any(role => role.Id == roleId),
            cancellationToken
        );

        return roleInUse;
    }

    public async Task<Role?> GetRoleByTypeAsync(string roleType, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleType == roleType, cancellationToken);

        return role;
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);

        return role;
    }

    public async Task<bool> IsRoleTypeExists(string roleType, CancellationToken cancellationToken = default)
    {
        bool roleTypeExists = await _context.Roles.AnyAsync(x => x.RoleType == roleType, cancellationToken);

        return roleTypeExists;
    }

    public async Task<bool> IsRoleNameExists(string roleName, CancellationToken cancellationToken = default)
    {
        bool roleNameExists = await _context.Roles.AnyAsync(x => x.RoleName == roleName, cancellationToken);

        return roleNameExists;
    }

    public async Task<bool> IsRoleExists(Guid id, CancellationToken cancellationToken = default)
    {
        bool roleExists = await _context.Roles.AnyAsync(x => x.Id == id, cancellationToken);

        return roleExists;
    }

        public async Task<(IReadOnlyList<Role> Roles, int TotalRecords)> GetPagedRolesAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        const string sql = """
            SELECT COUNT(*)
            FROM "Roles"
            WHERE "IsDeleted" = false;

            SELECT "Id", "RoleType", "RoleName", "Description", "CreatedAt", "UpdateAt", "IsDeleted"
            FROM "Roles"
            WHERE "IsDeleted" = false
            ORDER BY "CreatedAt", "Id"
            LIMIT @Limit OFFSET @Offset;
            """;

        DbConnection connection = _context.Database.GetDbConnection();

        using SqlMapper.GridReader grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                sql,
                new { Limit = pageSize, Offset = (pageNumber - 1) * pageSize },
                cancellationToken: cancellationToken
            )
        );

        int totalRecords = await grid.ReadSingleAsync<int>();
        List<Role> roles = (await grid.ReadAsync<Role>()).AsList();

        return (roles, totalRecords);
    }
}
