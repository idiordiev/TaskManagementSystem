using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Commands;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Infrastructure.Identity;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public AuthController(IIdentityService identityService, IMediator mediator)
    {
        _identityService = identityService;
        _mediator = mediator;
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
    public async Task<ActionResult<UserResponse>> Register([FromBody] CreateUserCommand createUserCommand,
        CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(createUserCommand, cancellationToken);

        return Ok(user);
    }
}