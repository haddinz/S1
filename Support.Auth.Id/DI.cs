using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repositories;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Features.Handler.Users;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Features.Handler.Auth;
using Support.Auth.Id.Features.Security;
using Support.Auth.Id.Services;
using Support.Notification.Client.Contract;
using Support.Notification.Client.Id.Services;

namespace Support.Auth.Id;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthModule
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Add Authentication and Authorization User and Password (costum)
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        // Add Repository
        services.AddScoped<IAuthRepositories, AuthRepositories>();

        // Add Security Services
        services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
        services.AddScoped<
            IEmailVerificationTokenGenerator,
            EmailVerificationTokenGenerator
        >();
        services.AddScoped<IAutEmailServices, AutEmailServices>();

        // Add Handler User
        services.AddScoped<ICommandQueryHandler<GetActiveUserQuery, UserProfileResponse>,GetActiveUserQueryHandler>();
        services.AddScoped<ICommandQueryHandler<GetUsersQuery, PagedResponse<UserProfileResponse>>,GetUsersQueryHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();

        // Add Handler Auth
        services.AddScoped<ICommandHandler<LoginCommand, AuthResponse>,LoginCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
        services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, AuthResponse>,RefreshTokenCommandHandler>();
        services.AddScoped<ICommandHandler<ChangesPasswordCommand>,ChangesPasswordCommandHandler>();
        services.AddScoped<ICommandHandler<VerifyEmailCommand>,VerifyEmailCommandHandler>();

        // Add Client Services
        services.AddScoped<INotificationClient, EmailSenderServices>();

        return services;
    }
}
