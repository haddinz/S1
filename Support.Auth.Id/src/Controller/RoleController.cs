using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Auth.Client.DTOs.Request;
using Support.Auth.Client.DTOs.Request.Role;
using Support.Auth.Client.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Extention;
using Support.Auth.Id.Features.Handler.Roles;
using Support.Auth.Id.Models.DTOs.Request;

namespace Support.Auth.Id.Controller;

[ApiController]
[Route("api/a/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ICommandHandler<CreateRoleCommand> _createRoleHandler;
    private readonly ICommandHandler<UpdateRoleCommand> _updateRoleHandler;
    private readonly ICommandHandler<DeleteRoleCommand> _deleteRoleHandler;
    private readonly ICommandHandler<AssignRoleCommand> _assignRoleHandler;
    private readonly ICommandHandler<RevokeRoleCommand> _revokeRoleHandler;
    private readonly ICommandQueryHandler<GetRoleByIdQuery, RoleResponse> _getRoleByIdHandler;
    private readonly ICommandQueryHandler<GetRolesQuery, PagedResponse<RoleResponse>> _getRolesHandler;

    public RoleController(
        ICommandHandler<CreateRoleCommand> createRoleHandler,
        ICommandHandler<UpdateRoleCommand> updateRoleHandler,
        ICommandHandler<DeleteRoleCommand> deleteRoleHandler,
        ICommandHandler<AssignRoleCommand> assignRoleHandler,
        ICommandHandler<RevokeRoleCommand> revokeRoleHandler,
        ICommandQueryHandler<GetRoleByIdQuery, RoleResponse> getRoleByIdHandler,
        ICommandQueryHandler<GetRolesQuery, PagedResponse<RoleResponse>> getRolesHandler
    )
    {
        _createRoleHandler = createRoleHandler;
        _updateRoleHandler = updateRoleHandler;
        _deleteRoleHandler = deleteRoleHandler;
        _assignRoleHandler = assignRoleHandler;
        _revokeRoleHandler = revokeRoleHandler;
        _getRoleByIdHandler = getRoleByIdHandler;
        _getRolesHandler = getRolesHandler;
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("{roleId:guid}")]
    public async Task<IActionResult> GetRoleById(Guid roleId, CancellationToken cancellationToken)
    {
        GetRoleByIdQuery query = new() { RoleId = roleId };

        RoleResponse role = await _getRoleByIdHandler.HandleAsync(query, cancellationToken);

        return Ok(ApiResponse.Success(role, "Role retrieved successfully"));
    }

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetRoles([FromQuery] PaginationRequest request, CancellationToken cancellationToken)
    {
        GetRolesQuery query = new()
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };

        PagedResponse<RoleResponse> roles = await _getRolesHandler.HandleAsync(query, cancellationToken);

        return Ok(roles);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        CreateRoleCommand command = new()
        {
            RoleType = request.RoleType,
            RoleName = request.RoleName,
            Description = request.Description,
        };

        await _createRoleHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success($"Role '{request.RoleName}' created successfully"));
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{roleId:guid}")]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        UpdateRoleCommand command = new()
        {
            RoleId = roleId,
            RoleType = request.RoleType,
            RoleName = request.RoleName,
            Description = request.Description,
        };

        await _updateRoleHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success("Role updated successfully"));
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        AssignRoleCommand command = new()
        {
            UserId = dto.UserId,
            RoleId = dto.RoleId,
        };

        await _assignRoleHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success($"Role assigned successfully to user '{dto.UserId}'"));
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("revoke-role")]
    public async Task<IActionResult> RevokeRole([FromBody] RevokeRoleRequest dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelStateExtention.ModelStateResponse(ModelState));
        }

        RevokeRoleCommand command = new()
        {
            UserId = dto.UserId,
            RoleId = dto.RoleId,
        };

        await _revokeRoleHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success($"Role revoked successfully from user '{dto.UserId}'"));
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{roleId:guid}")]
    public async Task<IActionResult> DeleteRole(Guid roleId, CancellationToken cancellationToken)
    {
        DeleteRoleCommand command = new() { RoleId = roleId };

        await _deleteRoleHandler.HandleAsync(command, cancellationToken);

        return Ok(ApiResponse.Success("Role deleted successfully"));
    }
}
