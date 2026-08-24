using System;
using System.Collections.Generic;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Repositories.Interfaces;

public interface IAuthRepositories
{
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUserByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetUserByRoleAsync(string roleName, CancellationToken cancellationToken = default);

    Task<bool> IsUserEmailExists(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameExists(string username, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<User> Users, int TotalPages)> GetPagedUsersAsync(int pageNumber, int pageSize);

    Task<User?> GetUserByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<User?> GetUserByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
}
