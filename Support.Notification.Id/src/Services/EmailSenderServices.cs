using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;
using Support.Notification.Id.Domain.ValueObject;

namespace Support.Notification.Client.Id.Services;

public class EmailSenderServices : INotificationClient
{
    private readonly ILogger<EmailSenderServices> _logger;
    private readonly SmtpOptions _options;

    public EmailSenderServices(ILogger<EmailSenderServices> logger, IOptions<SmtpOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task SendAsync(
        SendEmailRequest message,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit SendAsync at EmailSenderServices");

        MimeMessage email = new();

        email.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
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

    public async Task SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit SendEmailAsync at EmailSenderServices");

        MimeMessage email = new();

        email.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        email.To.Add(MailboxAddress.Parse(request.To));

        email.Subject = request.Subject;

        BodyBuilder bodyBuilder = new()
        {
            HtmlBody = request.HTMLBody
        };

        email.Body = bodyBuilder.ToMessageBody();

        SecureSocketOptions secureSocketOptions = _options.EnableSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(
            _options.Host,
            _options.Port,
            secureSocketOptions,
            cancellationToken
        );

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            await smtp.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        }

        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
