namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailBackgroundServices
{
    Task SendBackgroundEmailAsync(
        string email,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    );
}