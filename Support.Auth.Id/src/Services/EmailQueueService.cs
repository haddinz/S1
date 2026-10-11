using Hangfire;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Services;

public sealed class EmailQueueService : IEmailQueueService
{
    public async Task EnqueueForgotPassword(string email, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        BackgroundJob.Enqueue<IEmailBackgroundServices>
        (
            service => service.SendBackgroundEmailAsync(email, subject, htmlBody, cancellationToken)
        );
    }
}