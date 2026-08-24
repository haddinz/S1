using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Auth.Client.DTOs.Request;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Extention;
using Support.Auth.Id.Features.Handler.Auth;
using Support.Auth.Id.Models.DTOs;
using Support.Auth.Id.Models.DTOs.Request;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Services.Interfaces;

namespace Support.Auth.Id.Controller;

[ApiController]
[Route("api/a/[controller]")]
public sealed class AuthController : ControllerBase
{
   private readonly ICommandHandler<LoginCommand, AuthResponse> _loginHandler;
    private readonly ICommandHandler<LogoutCommand> _logoutHandler;
    private readonly ICommandHandler<RegisterCommand> _registerHandler;
    private readonly ICommandHandler<RefreshTokenCommand, AuthResponse> _refreshTokenHandler;
    private readonly ICommandHandler<ChangesPasswordCommand> _changesPasswordHandler;
    private readonly ICommandHandler<VerifyEmailCommand> _verifyEmailHandler;
    private readonly ICommandHandler<ResendVerifyEmailCommand> _resendVerification;
    private readonly ICommandHandler<ForgotPasswordCommand> _forgotPasswordHandler;

    public AuthController(
        ICommandHandler<LoginCommand, AuthResponse> loginHandler,
        ICommandHandler<LogoutCommand> logoutHandler,
        ICommandHandler<RegisterCommand> registerHandler,
        ICommandHandler<RefreshTokenCommand, AuthResponse> refreshTokenHandler,
        ICommandHandler<ChangesPasswordCommand> changesPassword,
        ICommandHandler<VerifyEmailCommand> verifyEmailHandler
,
        ICommandHandler<ResendVerifyEmailCommand> resendVerification,
        ICommandHandler<ForgotPasswordCommand> forgotPasswordHandler)
    {
        _loginHandler = loginHandler;
        _logoutHandler = logoutHandler;
        _registerHandler = registerHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _changesPasswordHandler = changesPassword;
        _verifyEmailHandler = verifyEmailHandler;
        _resendVerification = resendVerification;
        _forgotPasswordHandler = forgotPasswordHandler;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        RegisterCommand command = new()
        {
            Email = request.Email,
            FullName = request.FullName,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber,
            UserName = request.UserName,
        };

        await _registerHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success("User register successfully"));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        LoginCommand command = new()
        {
            Email = request.Email,
            IpAddress = GetIpAddress(),
            Password = request.Password,
            RememberMe = request.RememberMe,
        };

        AuthResponse response = await _loginHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success<AuthResponse>(response, "Successfully Login"));
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        RefreshTokenCommand command = new()
        {
            IpAddress = GetIpAddress(),
            RefreshToken = request.RefreshToken,
        };

        AuthResponse response = await _refreshTokenHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success<AuthResponse>(response, "Successfully Refresh Token"));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(
        ChangePasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        ChangesPasswordCommand command = new()
        {
            UserId = GetCurrentUserId(),
            IpAddress = GetIpAddress(),
            Password = request.Password,
            NewPassword = request.NewPassword,
        };

        await _changesPasswordHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success("Change Password Successfully"));
    }

    [AllowAnonymous]
    [HttpPost("change-password/forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        ForgotPasswordCommand command = new()
        {
            Email = request.Email
        };

        await _forgotPasswordHandler.HandleAsync(command, cancellationToken);
        return Ok(ApiResponse.Success("Forgot Password Successfully Send To Email")); 
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail(
        VerifyEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        VerifyEmailCommand command = new() { Token = request.Token };

        await _verifyEmailHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success("Verify Email Successfully"));
    }

    [AllowAnonymous]
    [HttpPost("verify-email/resend")]
    public async Task<ActionResult> ResendVerifyEmail(
        ResendVerifyEmailRequest request,
        CancellationToken cancellation
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        ResendVerifyEmailCommand command = new() { Email = request.Email };

        await _resendVerification.HandleAsync(command);

        return Ok(ApiResponse.Success("Resend Verify Email Successfully"));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        LogoutCommand command = new()
        {
            IpAddress = GetIpAddress(),
            RefreshTokenValue = request.RefreshToken,
        };

        await _logoutHandler.HandleAsync(command, cancellationToken);

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        // Better design = never get userid from request,
        // better from jwt and generate value because is more save

        string? userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("Invalid Token");

        return Guid.Parse(userId);
    }

    private string GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
