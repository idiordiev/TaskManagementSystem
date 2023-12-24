using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Tasks.Commands;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Application.Tasks.Queries;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/tasks")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll(int userId, [FromQuery] TaskFiltersModel filters,
        CancellationToken cancellationToken)
    {
        var query = new GetTasksByUserIdQuery { UserId = userId, Filters = filters };
        var tasks = await _mediator.Send(query, cancellationToken);

        return Ok(tasks);
    }
    
    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int taskId, CancellationToken cancellationToken)
    {
        var query = new GetTaskByIdQuery { TaskId = taskId };
        var task = await _mediator.Send(query, cancellationToken);

        if (task is null)
        {
            return NotFound();
        }
        
        return Ok(task);
    }
    
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(int userId, [FromBody] CreateTaskCommand createTaskCommand,
        CancellationToken cancellationToken)
    {
        createTaskCommand.UserId = userId;
        var task = await _mediator.Send(createTaskCommand, cancellationToken);

        return Created(nameof(GetById), task);
    }
    
    [HttpPut("{taskId:int}")]
    public async Task<ActionResult<TaskResponse>> Update(int taskId, [FromBody] UpdateTaskCommand updateTaskCommand,
        CancellationToken cancellationToken)
    {
        updateTaskCommand.TaskId = taskId;
        var task = await _mediator.Send(updateTaskCommand, cancellationToken);

        return Ok(task);
    }
    
    [HttpDelete("{taskId:int}")]
    public async Task<ActionResult> Delete(int taskId, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand { TaskId = taskId };
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}