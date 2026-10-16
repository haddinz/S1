using Support.Notification.Client.DTOs.Request;

namespace Support.Notification.Id.Services.Interfaces;

public interface IEmailBackgroundServices
{
    Task SendBackgroundEmailAsync(SendEmailRequest request);
}
