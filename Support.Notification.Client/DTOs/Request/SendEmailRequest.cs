using System.ComponentModel.DataAnnotations;

namespace Support.Notification.Client.DTOs.Request;

public sealed record SendEmailRequest
{
    [Required(ErrorMessage = "Email To Required")]
    public string To { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email Subject Required")]
    public string Subject { get; init; } = string.Empty;

    [Required(ErrorMessage = "Email HTMLBody Required")]
    public string HTMLBody { get; init; } = string.Empty;

    public string From { get; init; } = string.Empty;
    public string Cc { get; init; } = string.Empty;

    // public List<EmailAttachment> Attachments { get; init; } = [];
}
