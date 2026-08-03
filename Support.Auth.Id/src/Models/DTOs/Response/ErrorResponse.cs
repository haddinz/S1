namespace Support.Auth.Id.Models.DTOs.Response;

public sealed record ErrorResponse
{
    public string Message { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public string TraceId { get; init; } = string.Empty;

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
