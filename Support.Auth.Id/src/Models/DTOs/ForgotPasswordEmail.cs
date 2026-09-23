namespace Support.Auth.Id.Models;

public sealed record ForgotPasswordEmail(string Email, string FullName, string HTMLBody, string Token);