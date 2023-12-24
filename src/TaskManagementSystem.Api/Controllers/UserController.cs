using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        return Ok(users);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserEntity>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserEntity>> Create([FromBody] CreateUserContract createUserContract, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateAsync(createUserContract, cancellationToken);

            return Created(nameof(GetById), user);
        }
        catch (UserExistsException ex)
        {
            return BadRequest($"User with such email {createUserContract.Email} already exists");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserEntity>> Update(int id, [FromBody] UpdateUserContract updateUserContract, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(id, updateUserContract, cancellationToken);

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _userService.DeactivateAsync(id, cancellationToken);

        return Ok();
    }
}