using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Exceptions;
using Support.Auth.Id.Extention;
using Support.Auth.Id.Features.Handler.Users;
using Support.Auth.Id.Models.DTOs.Request;
using Support.Auth.Id.Models.DTOs.Response;

namespace Support.Auth.Id.Controller;

[ApiController]
[Route("api/a/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly ICommandQueryHandler<GetActiveUserQuery, UserProfileResponse> _getActiveUser;
    private readonly ICommandQueryHandler<GetUsersQuery,PagedResponse<UserProfileResponse>> _getUsers;
    private readonly ICommandHandler<UpdateUserCommand> _updateProfileHandler;

    public UserController
    (
        ICommandQueryHandler<GetActiveUserQuery, UserProfileResponse> getActiveUser,
        ICommandQueryHandler<GetUsersQuery, PagedResponse<UserProfileResponse>> getUsers,
        ICommandHandler<UpdateUserCommand> updateProfileHandler
    )
    {
        _getActiveUser = getActiveUser;
        _getUsers = getUsers;
        _updateProfileHandler = updateProfileHandler;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileResponse>> GetUserProfile()
    {
        Guid userId = GetCurrentUserId();

        UserProfileResponse? user = await _getActiveUser.HandleAsync(new GetActiveUserQuery(userId));

        return Ok(
            ApiResponse.Success<UserProfileResponse>(user, "User profile retrieved successfully")
        );
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult> UpdateUserProfile([FromBody] UserProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        Guid userId = GetCurrentUserId();
        await _updateProfileHandler.HandleAsync(
            new UpdateUserCommand(userId, request.FullName, request.PhoneNumber)
        );

        return Ok(ApiResponse.Success("Update user profile successfully"));
    }

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserProfileResponse>>> GetUsers(
        [FromQuery] PaginationRequest request
    )
    {
        PagedResponse<UserProfileResponse> result = await _getUsers.HandleAsync(
            new GetUsersQuery(request)
        );
        
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        string? userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("Invalid Token");

        return Guid.Parse(userId);
    }
}
