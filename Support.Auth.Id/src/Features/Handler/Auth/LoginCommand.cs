using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Domain.ValueObject;
using Microsoft.Extensions.Options;

namespace Support.Auth.Id.Features.Handler.Auth;

public class LoginCommand : ICommand<AuthResponse>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
    public bool RememberMe { get; init; }
}

public class LoginCommandHandler : ICommandHandler<LoginCommand, AuthResponse>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly IAuthRepositories _authRepo;
    private readonly IAppHasher _appHasher;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        ILogger<LoginCommandHandler> logger,
        IOptions<JwtSettings> jwtSettings,
        IAuthRepositories authRepo,
        IAppHasher appHasher,
        ITokenGenerator tokenGenerator
    )
    {
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
        _authRepo = authRepo;
        _appHasher = appHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit LoginCommandHandler");

        User? user =
            await _authRepo.GetUserByEmailAsync(command.Email, cancellationToken)
            ?? throw new UnauthorizedException("Invalid user or password");

        user.ValidateLogin();

        bool passwordValid = _appHasher.Verify(command.Password, user.PasswordHash);
        if (!passwordValid)
        {
            user.IncrementAccessFailed();
            await _authRepo.SaveChangesAsync(cancellationToken);

            throw new UnauthorizedException("Invalid user or password");
        }

        user.RecordSuccessfulLogin();

        string accessToken = _tokenGenerator.GeneratorAccessToken(user);
        RefreshToken refreshToken = _tokenGenerator.GeneratorRefreshToken(command.IpAddress);

        user.AddRefreshToken(refreshToken);

        await _authRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
        };
    }
}
