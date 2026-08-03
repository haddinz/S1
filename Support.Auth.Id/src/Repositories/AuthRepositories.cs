using Microsoft.EntityFrameworkCore;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repository.Data;

namespace Support.Auth.Id.Repositories;

public class AuthRepositories : IAuthRepositories
{
    private readonly AppDbContext _context;

    public AuthRepositories(AppDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        RefreshToken? refreshToken = await _context
            .RefreshTokens.Include(x => x.User)
                .ThenInclude(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);

        return refreshToken;
    }

    public async Task<User?> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _context
            .Users.Include(x => x.Roles)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _context
            .Users.Include(x => x.Roles)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user;
    }

    public async Task<User?> GetUserByNameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _context
            .Users.Include(x => x.Roles)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.UserName == username, cancellationToken);

        return user;
    }

    public async Task<Role?> GetUserByRoleAsync(
        string roleName,
        CancellationToken cancellationToken = default
    )
    {
        Role? role = await _context.Roles.FirstOrDefaultAsync(
            x => x.RoleName == roleName,
            cancellationToken
        );

        return role;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<(IReadOnlyList<User> Users, int TotalPages)> GetPagedUsersAsync(
        int pageNumber,
        int pageSize
    )
    {
        IQueryable<User> queryUser = _context.Users.AsNoTracking();
        int totalRecords = await queryUser.CountAsync();

        List<User> users = await queryUser
            .OrderBy(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalRecords);
    }

    public async Task<User?> GetUserByEmailVerificationTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        User? user = await _context
            .Users.Include(x => x.Roles)
            .Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(
                x => x.EmailVerification != null && x.EmailVerification.Token == token,
                cancellationToken: cancellationToken
            );

        return user;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
