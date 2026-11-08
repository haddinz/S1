using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repositories;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Features.Handler.Users;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Features.Handler.Auth;
using Support.Auth.Id.Features.Handler.Roles;
using Support.Auth.Id.Features.Security;
using Support.Auth.Client.DTOs.Response;

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
        services.AddScoped<IAppHasher, AppHasher>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        // Add Repository
        services.AddScoped<IAuthRepositories, AuthRepositories>();
        services.AddScoped<IRoleRepositories, RoleRepositories>();

        // Add Security Services
        services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();
        services.AddScoped(typeof(ISecureTokenGenerator<>), typeof(EmailTokenGenerator<>));

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
        services.AddScoped<ICommandHandler<ResendVerifyEmailCommand>, ResendVerifyEmailCommandHandler>();
        services.AddScoped<ICommandHandler<ForgotPasswordCommand>, ForgotPasswordCommandHandler>();
        services.AddScoped<ICommandHandler<ResetPasswordCommand>, ResetPasswordCommandHandler>();

        // Add Handler Role
        services.AddScoped<ICommandHandler<CreateRoleCommand>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateRoleCommand>, UpdateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand>, DeleteRoleCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRoleCommand>, AssignRoleCommandHandler>();
        services.AddScoped<ICommandHandler<RevokeRoleCommand>, RevokeRoleCommandHandler>();
        services.AddScoped<ICommandQueryHandler<GetRoleByIdQuery, RoleResponse>, GetRoleByIdQueryHandler>();
        services.AddScoped<ICommandQueryHandler<GetRolesQuery, PagedResponse<RoleResponse>>, GetRolesQueryHandler>();

        return services;
    }
}
