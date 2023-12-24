using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Infrastructure.Identity;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IUserService _userService;
    
    public AuthController(IIdentityService identityService, IUserService userService)
    {
        _identityService = identityService;
        _userService = userService;
    }
    
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> GetToken([FromBody] TokenRequest tokenRequest)
    {
        var token = await _identityService.GetTokenAsync(tokenRequest.Email, tokenRequest.Password);
        
        return Ok(new TokenResponse { AccessToken = token });
    }
    
    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Register([FromBody] CreateUserContract createUserContract, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(createUserContract, cancellationToken);
        
        return Ok(user);
    }
}