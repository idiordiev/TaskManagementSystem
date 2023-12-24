using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Users.Commands;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Application.Users.Queries;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetActiveUsersQuery(), cancellationToken);

        return Ok(users);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserEntity>> GetById(int id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var user = await _mediator.Send(query, cancellationToken);
        
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserEntity>> Create([FromBody] CreateUserCommand createUserCommand, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _mediator.Send(createUserCommand, cancellationToken);

            return Created(nameof(GetById), user);
        }
        catch (UserExistsException ex)
        {
            return BadRequest($"User with such email {createUserCommand.Email} already exists");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserEntity>> Update(int id, [FromBody] UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
    {
        updateUserCommand.Id = id;
        var user = await _mediator.Send(updateUserCommand, cancellationToken);

        return Ok(user);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand { UserId = id };
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}