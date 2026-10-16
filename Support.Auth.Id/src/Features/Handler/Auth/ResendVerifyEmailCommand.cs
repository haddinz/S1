using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Constans;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Features.Helper;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;

namespace Support.Auth.Id.Features.Handler.Auth;

public class ResendVerifyEmailCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

public class ResendVerifyEmailCommandHandler : ICommandHandler<ResendVerifyEmailCommand>
{
    private readonly ILogger<ResendVerifyEmailCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IEmailQueueServiceClient _emailQueue;
    private readonly ISecureTokenGenerator<EmailVerificationToken> _emailTokenGenerator;
    private readonly FrontendSettings _frontendSettings;
    private readonly IAppHasher _appHasher;

    public ResendVerifyEmailCommandHandler
    (
        ILogger<ResendVerifyEmailCommandHandler> logger,
        IAuthRepositories authRepo,
        ISecureTokenGenerator<EmailVerificationToken> emailTokenGenerator,
        IOptions<FrontendSettings> frontendSettings,
        IAppHasher appHasher,
        IEmailQueueServiceClient emailQueue
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _emailTokenGenerator = emailTokenGenerator;
        _frontendSettings = frontendSettings.Value;
        _appHasher = appHasher;
        _emailQueue = emailQueue;
    }

    public async Task HandleAsync
    (
        ResendVerifyEmailCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit ResendVerifyEmailCommandHandler at Handler Support Auth");

        User? user = await _authRepo.GetUserByEmailAsync(command.Email, cancellationToken)
            ?? throw new BusinessValidationException("Email is invalid or not registered.");

        if (user.IsEmailVerified)
            throw new ConflictException("User with this email already verified.");

        string htmlBody = await _authRepo.ExecuteInTransactionAsync(async cancellationToken =>
        {
            SecureTokenGenerator<EmailVerificationToken> emailVerificationToken = _emailTokenGenerator.Generate(TimeSpan.FromMinutes(30));
            string tokenHash = _appHasher.HashToken(emailVerificationToken.Token);

            user.SetEmailVerificationToken(tokenHash, emailVerificationToken.ExpiresAt);

            string verificationUrl =
                $"{_frontendSettings.BaseUrl.TrimEnd('/')}/verify-email/resend?token={Uri.EscapeDataString(emailVerificationToken.Token)}";

            return HtmlTemplateEngine.Render(Template.Name.VerifyEmail, new()
            {
                { "FULL_NAME", user.FullName },
                { "VERIFICATION_URL", verificationUrl }
            });
        }, cancellationToken);

        
        SendEmailRequest sendEmailRequest = new()
        {
            To = user.Email,
            Subject = "Verify Email",
            HTMLBody = htmlBody,
        };

        await _emailQueue.EnqueueEmailAsync(sendEmailRequest, cancellationToken);
    }
}
