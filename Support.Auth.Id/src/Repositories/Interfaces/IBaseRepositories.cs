using Microsoft.EntityFrameworkCore.Storage;

namespace Support.Auth.Id.Repositories;

public interface IBaseRepositories
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<T> ExecuteInTransactionAsync<T>
    (
        Func<CancellationToken, Task<T>> work, CancellationToken cancellationToken = default
    );

    Task ExecuteInTransactionAsync
    (
        Func<CancellationToken, Task> work, CancellationToken cancellationToken = default
    );
}