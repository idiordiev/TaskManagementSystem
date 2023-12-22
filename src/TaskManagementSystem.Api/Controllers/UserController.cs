using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
//[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly UserManager<Account> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(IUserService userService, UserManager<Account> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userService = userService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("grant-admin")]
    public async Task<IActionResult> GrantAdmin()
    {
        var user = await _userManager.Users.Where(x => x.UserId == 2).FirstOrDefaultAsync();
    
        await _roleManager.CreateAsync(new IdentityRole("User"));
        await _roleManager.CreateAsync(new IdentityRole("Admin"));
        
        await _userManager.AddToRoleAsync(user, "Admin");
    
        return Ok();
    }
}