using System.Threading;
using Support.Notification.Client.DTOs.Request;

namespace Support.Notification.Client.Contract;

public interface INotificationClient
{
    Task SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken = default);
}
