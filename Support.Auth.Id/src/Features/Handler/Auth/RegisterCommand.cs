using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Constans;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Features.Helper;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;
using Support.Notification.Client.Contract;
using Support.Notification.Client.DTOs.Request;

namespace Support.Auth.Id.Features.Handler.Auth;

public class RegisterCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly ILogger<RegisterCommandHandler> _logger;
    private readonly IAuthRepositories _authRepo;
    private readonly IAppHasher _appHasher;
    private readonly IPasswordPolicyValidator _validator;
    private readonly IEmailQueueServiceClient _emailQueue;
    private readonly ISecureTokenGenerator<EmailVerificationToken> _emailTokenGenerator;
    private readonly FrontendSettings _options;

    public RegisterCommandHandler
    (
        ILogger<RegisterCommandHandler> logger,
        IAuthRepositories authRepo,
        IAppHasher appHasher,
        IPasswordPolicyValidator validator,
        ISecureTokenGenerator<EmailVerificationToken> emailTokenGenerator,
        IOptions<FrontendSettings> options,
        IEmailQueueServiceClient emailQueue
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _appHasher = appHasher;
        _validator = validator;
        _emailTokenGenerator = emailTokenGenerator;
        _options = options.Value;
        _emailQueue = emailQueue;
    }

    public async Task HandleAsync
    (
        RegisterCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit RegisterCommandHandler at AuthServices");

        if (await _authRepo.IsUserEmailExists(command.Email, cancellationToken))
            throw new ConflictException("Email already exists");

        if (await _authRepo.IsUsernameExists(command.UserName, cancellationToken))
            throw new ConflictException("Username already use");

        string htmlBody = await _authRepo.ExecuteInTransactionAsync(async ct =>
        {
            // stop first for testing while register
            // _validator.Validate(command.Password);
            string passwordHash = _appHasher.Hash(command.Password);
            User user = User.Register(command.Email, command.FullName, command.UserName, passwordHash);

            Role? roleUser = await _authRepo
                .GetUserByRoleAsync(RoleEnum.User.ToString(), cancellationToken) 
                ?? throw new NotFoundException("Default role not found");
            user.AssignRole(roleUser);

            SecureTokenGenerator<EmailVerificationToken> emailVerivicationToken = _emailTokenGenerator.Generate(TimeSpan.FromMinutes(30));
            string tokenHash = _appHasher.HashToken(emailVerivicationToken.Token);

            user.SetEmailVerificationToken(tokenHash, emailVerivicationToken.ExpiresAt);

            string verificationUrl =
                $"{_options.BaseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(emailVerivicationToken.Token)}";

            await _authRepo.AddAsync(user, cancellationToken);

            return HtmlTemplateEngine.Render(
                Template.Name.VerifyEmail,
                new() { { "FULL_NAME", user.FullName }, { "VERIFICATION_URL", verificationUrl } }
            );

        }, cancellationToken);

        SendEmailRequest sendEmailRequest = new()
        {
            To = command.Email,
            Subject = "Register",
            HTMLBody = htmlBody,
        };

        await _emailQueue.EnqueueEmailAsync(sendEmailRequest, cancellationToken);
    }
}
