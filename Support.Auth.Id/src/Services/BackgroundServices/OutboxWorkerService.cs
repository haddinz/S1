using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Constans;
using System.Text.Json;
using Support.Auth.Id.Models;
using Support.Auth.Id.Repositories;

namespace Support.Auth.Id.Services.BackgroundServices;

public class OutboxWorkerService : BackgroundService
{
    private readonly ILogger<OutboxWorkerService> _logger;
    private readonly IServiceScopeFactory _scoprFactory;
    private readonly IAuthEmailServices _emailServices;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
    private readonly int _batchSize = 10;
    private readonly int _maxRetryCount = 5;

    public OutboxWorkerService(ILogger<OutboxWorkerService> logger, IServiceScopeFactory scoprFactory, IAuthEmailServices emailServices)
    {
        _logger = logger;
        _scoprFactory = scoprFactory;
        _emailServices = emailServices;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> Hit ExecuteAsync at Background Services");

        while(!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("--> Critical error occurred while executing outbox with an error {ex}", ex);
            }

            await Task.Delay(_interval, cancellationToken);
        }

        _logger.LogInformation("--> Hit Background Services Stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _scoprFactory.CreateScope();
        OutboxWorkerRepositories _outboxRepo = scope.ServiceProvider.GetRequiredService<OutboxWorkerRepositories>();
        
        IReadOnlyList<OutboxMessage> penddingMessage = 
            await _outboxRepo.GetMessagesStatusAsync(_batchSize, Message.Status.Pending, cancellationToken);

        if (!penddingMessage.Any())
            return;
        
        _logger.LogInformation("--> Processing {Count} outbox messages", penddingMessage.Count());

        foreach (OutboxMessage message in penddingMessage)
        {
            try
            {
                AuthSendEmail payload = JsonSerializer.Deserialize<AuthSendEmail>(message.Payload)
                    ?? throw new InvalidOperationException("Invalid payload format");

                await _emailServices.SendEmailAsync(payload, cancellationToken);

                message.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--> Failed to process message {MessageId}", message.Id);

                message.HandleFailure(ex.Message, _maxRetryCount);
                if (message.IsPermanentlyFailed)
                {
                    _logger.LogError("Message {MessageId} has permanently failed after max retries.", message.Id);

                    // TODO: Sending notifikasi to admin (WA/Telegram/Email)
                    await NotifyAdminAsync(message, ex);
                }
            }

            await _outboxRepo.UpdateAsync(message, cancellationToken);
        }

        await _outboxRepo.SaveChangesAsync(cancellationToken);
    }

    // Feature nice to have
    private async Task NotifyAdminAsync(OutboxMessage message, Exception ex)
    {
        // TODO: Implement notifikasi admin
        // Contoh: Kirim Slack, Email, atau log ke sistem monitoring
        _logger.LogError
        (
            "ADMIN ALERT: Outbox message {MessageId} permanently failed. Error: {Error}", 
            message.Id, ex.Message
        );
        
        // I Can add this fot feature, nice to have:
        // await _slackService.SendAlertAsync($"Outbox {message.Id} failed: {ex.Message}");
        // await _emailSender.SendAdminAlertAsync($"Outbox {message.Id} permanently failed");
    }
}