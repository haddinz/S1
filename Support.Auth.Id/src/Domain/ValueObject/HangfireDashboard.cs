namespace Support.Auth.Id.Domain.ValueObject;

public sealed class HangfireDashboard
{
    public const string SectionName = "HangfireDashboard";

    public string Login { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool Ssl { get; init; }
    public bool SslRedirect { get; init; }
}
