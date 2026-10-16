using System.ComponentModel.DataAnnotations;

namespace Support.Notification.Client.DTOs;

public sealed record EmailAttachment
{
    [Required(ErrorMessage = "Attachment FileName Required")]
    [StringLength(255, ErrorMessage = "Attachment FileName max 255 characters")]
    public string FileName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Attachment ContentType Required")]
    [RegularExpression(
        @"^[\w.+-]+/[\w.+-]+$",
        ErrorMessage = "Attachment ContentType is not a valid MIME type"
    )]
    public string ContentType { get; init; } = "application/octet-stream";

    [Required(ErrorMessage = "Attachment Content Required")]
    [MinLength(1, ErrorMessage = "Attachment content cannot be empty")]
    [MaxLength(10 * 1024 * 1024, ErrorMessage = "Attachment content cannot exceed 10 MB")]
    public byte[] Content { get; init; } = [];
}