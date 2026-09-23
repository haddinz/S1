using System.ComponentModel.DataAnnotations;
using Support.Auth.Id.Constans;

namespace Support.Auth.Id.Domain.Entity;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = Message.Status.Pending;
    public int RetryCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }

    public string? LastError { get; set; }
    public bool IsPermanentlyFailed => Status == Message.Status.Failed;

    private OutboxMessage()
        : base() { }

    public void MarkAsProcessed()
    {
        Status = Message.Status.Completed;
        ProcessedAt = DateTime.UtcNow;
        LastError = null;
    }

    public void HandleFailure(string errorMessage, int maxRetryCount)
    {
        RetryCount++;
        LastError = errorMessage;

        if (RetryCount >= maxRetryCount)
            Status = Message.Status.Failed;
    }
}
