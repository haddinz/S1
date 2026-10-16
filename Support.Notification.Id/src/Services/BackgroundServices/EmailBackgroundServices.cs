using Hangfire;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;
using Support.Notification.Id.Services.Interfaces;

namespace Support.Notification.Id.Services.BackgroundServices;

[Queue("email")]
public class EmailBackgroundServices : IEmailBackgroundServices
{
    private readonly ILogger<EmailBackgroundServices> _logger;
    private readonly INotificationClient _emailSender;

    public EmailBackgroundServices
    (
        ILogger<EmailBackgroundServices> logger,
        INotificationClient emailSender
    )
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    [JobDisplayName("Send Email")]
    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 10, 30, 60, 120, 300 })]
    public async Task SendBackgroundEmailAsync(SendEmailRequest request)
    {
        _logger.LogInformation("--> Hit Hangfire: Sending email to {Email}", request.To);

        await _emailSender.SendEmailAsync(request, CancellationToken.None);
    }
}
