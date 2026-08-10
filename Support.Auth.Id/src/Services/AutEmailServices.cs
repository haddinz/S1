using Support.Auth.Id.Services.Interfaces;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;

namespace Support.Auth.Id.Services;

public class AutEmailServices : IAutEmailServices
{
    private readonly ILogger<AutEmailServices> _logger;
    private readonly INotificationClient _notificationClient;

    public AutEmailServices(INotificationClient notificationClient, ILogger<AutEmailServices> logger)
    {
        _logger = logger;
        _notificationClient = notificationClient;
    }

    public async Task SendEmailVerificationAsync(string email, string fullName, string HTMLBody, string token)
    {
        _logger.LogInformation("--> Hit SendEmailVerificationAsync at AutEmailServices");

        await _notificationClient.SendEmailAsync
        (
            new SendEmailRequest
            {
                To = email,
                Subject = "Verify Email",
                HTMLBody = HTMLBody
            }
        );
    }
}