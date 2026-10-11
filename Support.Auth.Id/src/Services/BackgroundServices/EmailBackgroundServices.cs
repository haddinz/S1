using Hangfire;
using Support.Auth.Id.Models;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Services.BackgroundServices;

public class EmailBackgroundServices : IEmailBackgroundServices
{
    private readonly ILogger<EmailBackgroundServices> _logger;
    private readonly IAuthEmailServices _emailSender;

    public EmailBackgroundServices
    (
        ILogger<EmailBackgroundServices> logger,
        IAuthEmailServices emailSender
    )
    {
        _logger = logger;
        _emailSender = emailSender;
    }

    [JobDisplayName("Send Forgot Password Email to {0}")]
    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 10, 30, 60, 120, 300 })]
    public async Task SendBackgroundEmailAsync(
        string email,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit Hangfire: Sending email to {Email}", email);

        AuthSendEmail authSendEmail = new()
        {
            To = email,
            Subject = subject,
            HTMLBody = htmlBody,
        };

        await _emailSender.SendEmailAsync(authSendEmail, cancellationToken);
    }
}