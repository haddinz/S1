using Hangfire;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;
using Support.Notification.Id.Services.Interfaces;

namespace Support.Notification.Id.Services;

public sealed class EmailQueueService : IEmailQueueServiceClient
{
    private readonly ILogger _logger;

    public EmailQueueService(ILogger logger)
    {
        _logger = logger;
    }


    public Task EnqueueEmailAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit EnqueueEmailAsync at EmailQueueService");
        
        BackgroundJob.Enqueue<IEmailBackgroundServices>
        (
            service => service.SendBackgroundEmailAsync(request)
        );

        return Task.CompletedTask;
    }
}
