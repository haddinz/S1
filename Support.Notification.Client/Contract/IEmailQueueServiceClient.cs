using Support.Notification.Client.DTOs.Request;

namespace Support.Notification.Client.Contract;

public interface IEmailQueueServiceClient
{
    Task EnqueueEmailAsync(SendEmailRequest request, CancellationToken cancellationToken = default);
}
