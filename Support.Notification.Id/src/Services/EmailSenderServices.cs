using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;
using Support.Notification.Id.Domain.ValueObject;

namespace Support.Notification.Client.Id.Services;

public class EmailSenderServices : INotificationClient
{
    private readonly Logger<EmailSenderServices> _logger;
    private readonly SmtpOptions _options;

    public EmailSenderServices(Logger<EmailSenderServices> logger, SmtpOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task SendAsync(
        EmailMessageRequest message,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit SendAsync at EmailSenderServices");

        MimeMessage email = new();

        email.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        email.To.Add(MailboxAddress.Parse(message.To));

        email.Subject = message.Subject;
        email.Body = new TextPart("html") { Text = message.HTMLBody };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(
            _options.Host,
            _options.Port,
            _options.EnableSsl,
            cancellationToken
        );

        await smtp.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        await smtp.SendAsync(email, cancellationToken);

        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
