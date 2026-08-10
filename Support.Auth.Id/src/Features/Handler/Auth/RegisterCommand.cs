using System.Net;
using Microsoft.Extensions.Options;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Features.Helper;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;

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
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordPolicyValidator _validator;
    private readonly IAutEmailServices _emailSender;
    private readonly IEmailVerificationTokenGenerator _emailTokenGenerator;
    private readonly FrontendSettings _options;

    public RegisterCommandHandler(
        ILogger<RegisterCommandHandler> logger,
        IAuthRepositories authRepo,
        IPasswordHasher passwordHasher,
        IPasswordPolicyValidator validator,
        IAutEmailServices emailSender,
        IEmailVerificationTokenGenerator emailTokenGenerator,
        IOptions<FrontendSettings> options
    )
    {
        _logger = logger;
        _authRepo = authRepo;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _emailSender = emailSender;
        _emailTokenGenerator = emailTokenGenerator;
        _options = options.Value;
    }

    public async Task HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _logger.LogInformation("--> Hit RegisterCommandHandler at AuthServices");

        User? existingEmail = await _authRepo.GetUserByEmailAsync(command.Email, cancellationToken);
        if (existingEmail is not null)
        {
            throw new ConflictException("Email already exists");
        }

        _validator.Validate(command.Password);
        string passwordHash = _passwordHasher.HashPassword(command.Password);

        User user = User.Register(command.Email, command.FullName, command.UserName, passwordHash);

        Role? roleUser =
            await _authRepo.GetUserByRoleAsync(RoleEnum.User.ToString(), cancellationToken)
            ?? throw new NotFoundException("Default role not found.");

        user.AssignRole(roleUser);

        EmailVerificationToken emailVerivicationToken = _emailTokenGenerator.Generate();
        user.SetEmailVerificationToken(
            emailVerivicationToken.Token,
            emailVerivicationToken.ExpiresAt
        );

        await _authRepo.AddAsync(user, cancellationToken);
        await _authRepo.SaveChangesAsync(cancellationToken);

        string verificationUrl =
            $"{_options.BaseUrl.TrimEnd('/')}/verify-email?token={Uri.EscapeDataString(emailVerivicationToken.Token)}";

        var htmlBody = HtmlTemplateEngine.Render("VerivyEmail.html", new()
        {
            { "FULL_NAME", user.FullName },
            { "VERIFICATION_URL", verificationUrl }
        });

        // dont backward, email just sending if save data to db success
        await _emailSender.SendEmailVerificationAsync(
            user.Email,
            user.FullName,
            htmlBody,
            emailVerivicationToken.Token
        );
    }
}
