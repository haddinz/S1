using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Support.Auth.Id.Models.Dtos.Request;
using Support.Auth.Id.Models.Dtos.Response;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Models.Enum;

namespace Support.Auth.Id.Controller;

[ApiController]
[Route("api/a/[controller]")]
public class RoleController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleController(
        ILogger logger,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager
    )
    {
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoleById(Guid roleID)
    {
        _logger.LogInformation("--> Hit GetRoleById");
        // var roleId = await _roleManager.GetRoleIdAsync(roleID.ToString());

        return Ok();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        _logger.LogInformation("--> Hit create role");
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest($"Role {roleName} already exist");
        }

        IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));
        if (!result.Succeeded)
        {
            return BadRequest($"{result.Errors}");
        }

        return Ok($"Successfully {roleName} creating Role");
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest dto)
    {
        _logger.LogInformation("--> Hit assign role");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        User? user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return NotFound($"User with email {dto.Email} is not found");
        }

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok($"Role {dto.RoleName} assigned to user {dto.Email} successfully");
    }
}
