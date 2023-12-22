using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    
    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponse>> GetToken([FromBody] TokenRequest tokenRequest)
    {
        var token = await _identityService.GetTokenAsync(tokenRequest.Email, tokenRequest.Password);
        
        return Ok(new TokenResponse { AccessToken = token });
    }
}