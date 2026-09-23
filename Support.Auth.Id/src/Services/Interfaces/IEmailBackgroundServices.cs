namespace Support.Auth.Id.Services.Interfaces;

public interface IEmailBackgroundServices
{
    Task SendBackgroundEmailAsycn(
        string email, 
        string fullName, 
        string subject,
        string templateSubject,
        string urlLink, 
        string plainToken,
        CancellationToken cancellationToken = default);
}