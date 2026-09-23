namespace Support.Auth.Id.Models;

public sealed record AuthSendEmail
{
    public string To { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string HTMLBody { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;
    public string Cc { get; init; } = string.Empty;
}