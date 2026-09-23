using Support.Auth.Id.Models;
using Support.Auth.Id.Services.Interfaces;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;

namespace Support.Auth.Id.Services;

public class AuthEmailServices : IAuthEmailServices
{
    private readonly ILogger<AuthEmailServices> _logger;
    private readonly INotificationClient _notificationClient;

    public AuthEmailServices(INotificationClient notificationClient, ILogger<AuthEmailServices> logger)
    {
        _logger = logger;
        _notificationClient = notificationClient;
    }

    public async Task SendEmailAsync(AuthSendEmail email, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit SendEmailAsync at AuthEmailServices");

        await _notificationClient.SendEmailAsync
        (
            new SendEmailRequest
            {
                To = email.To,
                Subject = email.Subject,
                HTMLBody = email.HTMLBody
            },
            cancellationToken
        );
    }

    public async Task SendEmailVerificationAsync(string email, string fullName, string HTMLBody, string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit SendEmailVerificationAsync at AuthEmailServices");

        await SendEmailAsync(email, HTMLBody, cancellationToken);
    }

    private async Task SendEmailAsync(string email, string HTMLBody, CancellationToken cancellationToken = default)
    {
        await _notificationClient.SendEmailAsync
        (
            new SendEmailRequest
            {
                To = email,
                Subject = "Verify Email",
                HTMLBody = HTMLBody
            },
            cancellationToken
        );
    }
}