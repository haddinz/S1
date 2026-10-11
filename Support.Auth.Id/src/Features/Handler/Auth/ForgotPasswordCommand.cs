using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Constans;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Features.Helper;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Features.Handler.Auth;

public sealed class ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IEmailQueueService _emailQueue;
    private readonly ISecureTokenGenerator<ForgotPasswordToken> _tokenGenerator;
    private readonly IAppHasher _appHasher;
    private readonly FrontendSettings _options;

    public ForgotPasswordCommandHandler(
        ILogger<ForgotPasswordCommandHandler> logger,
        IAuthRepositories authRepo,
        IEmailQueueService emailQueue,
        IOptions<FrontendSettings> options,
        ISecureTokenGenerator<ForgotPasswordToken> tokenGenerator,
        IAppHasher appHasher
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _emailQueue = emailQueue;
        _options = options.Value;
        _tokenGenerator = tokenGenerator;
        _appHasher = appHasher;
    }

    public async Task HandleAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit ForgotPasswordCommandHandler at Support Auth");

        string email = command.Email.Trim().ToLowerInvariant();
        User? user = await _authRepo.GetUserByEmailAsync(email, cancellationToken);

        // Security: do not reveal whether an account exists
        if (user is null)
        {
            _logger.LogInformation("Account not found; responding as if the reset email was sent");
            return;
        }

        if (!user.IsActive)
        {
            _logger.LogInformation("Account not activated yet; no reset email queued");
            return;
        }

        string htmlBody = await _authRepo.ExecuteInTransactionAsync(async ct =>
        {
            SecureTokenGenerator<ForgotPasswordToken> passwordResetToken = _tokenGenerator.Generate(TimeSpan.FromMinutes(30));

            string verificationUrl =
                $"{_options.BaseUrl.TrimEnd('/')}/forgot-password?token={Uri.EscapeDataString(passwordResetToken.Token)}";

            user.SetPasswordResetToken(_appHasher.HashToken(passwordResetToken.Token), passwordResetToken.ExpiresAt);

            // Rendered inside the transaction: a missing template throws here → rollback, job never queued.
            return HtmlTemplateEngine.Render(
                Template.Name.ForgotPassword,
                new() { { Template.Keys.FullName, user.FullName }, { Template.Keys.URL, verificationUrl } }
            );

        }, cancellationToken);

        // Queued only after the token is committed, so an email never references a rolled-back token.
        await _emailQueue.EnqueueForgotPassword(user.Email, "Forgot Password", htmlBody, cancellationToken);
    }
}