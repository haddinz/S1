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
}