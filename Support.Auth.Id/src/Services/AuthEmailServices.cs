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

    public async Task SendEmailVerificationAsync(string email, string fullName, string HTMLBody, string token)
    {
        _logger.LogInformation("--> Hit SendEmailVerificationAsync at AuthEmailServices");

        await SendEmailAsync(email, fullName, HTMLBody, token);
    }

    public async Task SendForgotPasswordAsync(string email, string fullName, string HTMLBody, string token)
    {
        _logger.LogInformation("--> Hit SendEmailForgotPassword at AuthEmailServices");

        await SendEmailAsync(email, fullName, HTMLBody, token);
    }

    private async Task SendEmailAsync(string email, string fullName, string HTMLBody, string token)
    {
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