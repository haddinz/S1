namespace Support.Notification.Id.Domain.ValueObject;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } 
    public int Port { get; init; }
    public string Username { get; init; }
    public string Password { get; init; }
    public string FromEmail { get; init; }
    public string FromName { get; init; }
    public bool EnableSsl { get; init; } = true;
}
