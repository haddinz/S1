using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Repositories.Interfaces;

public interface IOutboxWorkerRepositories
{
    Task<List<OutboxMessage>> GetMessagesStatusAsync(int batchSize, string status, CancellationToken cancellationToken = default);
    Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}