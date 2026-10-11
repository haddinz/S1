namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailQueueService
{
    Task EnqueueForgotPassword(string email, string subject, string htmlBody, CancellationToken cancellationToken = default);
}