namespace Support.Auth.Id.Models.DTOs;

public sealed record PasswordResetToken(string Token, DateTime ExpiresAt);
