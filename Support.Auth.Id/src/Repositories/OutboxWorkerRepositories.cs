using Microsoft.EntityFrameworkCore;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repository.Data;

namespace Support.Auth.Id.Repositories;

public class OutboxWorkerRepositories : IOutboxWorkerRepositories
{
    private readonly AppDbContext _context;

    public OutboxWorkerRepositories(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OutboxMessage>> GetMessagesStatusAsync(int batchSize, string status, CancellationToken cancellationToken = default)
    {
        List<OutboxMessage> penddingMessage = await _context.OutboxMessages
            .Where(x => x.Status == status)
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
        
        return penddingMessage;
    }

    public Task UpdateAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        _context.OutboxMessages.Update(message);
        
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}