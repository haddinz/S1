using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Domain.ValueObject;

namespace Support.Auth.Id.Features.Handler.Auth;

public class RefreshTokenCommand : ICommand<AuthResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
}

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly JwtSettings _jwtSettings;
    private readonly IAuthRepositories _authRepo;
    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokenCommandHandler(ILogger<RefreshTokenCommandHandler> logger, IAuthRepositories authRepo, ITokenGenerator tokenGenerator, JwtSettings jwtSettings)
    {
        _logger = logger;
        _authRepo = authRepo;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("--> Hit RefreshTokenCommandHandler");

        RefreshToken? storedToken =
            await _authRepo.GetRefreshTokenAsync(command.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedException("Refresh token expired or revoked");

        User user = storedToken.User;
        user.ValidateLogin();

        RefreshToken newRefreshToken = _tokenGenerator.GeneratorRefreshToken(command.IpAddress);

        storedToken.Revoke(command.IpAddress, newRefreshToken.Token);

        user.AddRefreshToken(newRefreshToken);

        string accessToken = _tokenGenerator.GeneratorAccessToken(user);

        await _authRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
        };
    }
}