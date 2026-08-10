namespace Support.Auth.Id.Services.Interfaces;

public interface IAutEmailServices
{
    Task SendEmailVerificationAsync(string email, string fullName, string htmlBody, string token);
}
