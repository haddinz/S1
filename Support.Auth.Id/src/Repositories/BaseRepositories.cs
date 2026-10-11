using Microsoft.EntityFrameworkCore.Storage;
using Support.Auth.Id.Repository.Data;

namespace Support.Auth.Id.Repositories;

public class BaseRepositories : IBaseRepositories
{
    protected readonly AppDbContext _context;

    public BaseRepositories(AppDbContext context) => _context = context;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> work, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            T result = await work(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken cancellationToken = default) 
        =>
            ExecuteInTransactionAsync(
                async ct =>
                {
                    await work(ct);
                    return true;
                },
            cancellationToken
    );
}
