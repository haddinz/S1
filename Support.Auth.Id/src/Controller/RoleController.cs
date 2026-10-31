using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Support.Auth.Client.DTOs.Request;
using Support.Auth.Client.DTOs.Request.Role;
using Support.Auth.Client.DTOs.Response;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Domain.Entity;
using Support.Auth.Id.Extention;
using Support.Auth.Id.Features.Handler.Roles;

namespace Support.Auth.Id.Controller;

[ApiController]
[Route("api/a/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ICommandHandler<CreateRoleCommand> _createRoleHandler;

    public RoleController(ICommandHandler<CreateRoleCommand> createRoleHandler)
    {
        _createRoleHandler = createRoleHandler;
    }

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetRoleById(Guid roleID)
    {
        // var roleId = await _roleManager.GetRoleIdAsync(roleID.ToString());

        return Ok();
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

        return Ok(ApiResponse.Success("User register successfully"));
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest dto)
    {



        return Ok($"Role {dto.RoleName} assigned to user {dto.Email} successfully");
    }
}
