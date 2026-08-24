namespace Support.Auth.Id.Domain.ValueObject;

public sealed class PasswordReset
{
    public string Token { get; private set; } = string.Empty;

    public DateTime? ExpiresAt { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsUsed => UsedAt.HasValue;

    private PasswordReset() { }

    internal PasswordReset(string token, DateTime expiresAt)
    {
        Token = token;
        ExpiresAt = expiresAt;
    }

    internal void MarkAsUsed()
    {
        ExpiresAt = null;
        Token = string.Empty;
        UsedAt = DateTime.UtcNow;
    }
}
