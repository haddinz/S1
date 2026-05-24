using Microsoft.AspNetCore.Identity;
using Support.Auth.Id.Models.Entity;

namespace Support.Auth.Id.Services;

public class RoleService
{
    private readonly ILogger _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public RoleService(
        ILogger<RoleService> logger,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager
    )
    {
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task GetAllRole()
    {
        try
        {
            // var role = await _roleManager.Roles();
        }
        catch (System.Exception)
        {
            throw;
        }
    }
}
