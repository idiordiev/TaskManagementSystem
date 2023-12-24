using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Subtasks.Commands;
using TaskManagementSystem.Application.Subtasks.Models;
using TaskManagementSystem.Application.Subtasks.Queries;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/tasks/{taskId:int}/subtasks")]
[Authorize]
public class SubtaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubtaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubtaskResponse>>> GetByTaskId(int taskId,
        CancellationToken cancellationToken)
    {
        var query = new GetSubtasksByTaskIdQuery { TaskId = taskId };
        var subtasks = await _mediator.Send(query, cancellationToken);

        return Ok(subtasks);
    }

    [HttpGet("{subtaskId:int}")]
    public async Task<ActionResult<SubtaskResponse>> GetById(int subtaskId, CancellationToken cancellationToken)
    {
        var query = new GetSubtaskByIdQuery { SubtaskId = subtaskId };
        var subtask = await _mediator.Send(query, cancellationToken);

        if (subtask is null)
        {
            return NotFound();
        }

        return Ok(subtask);
    }

    [HttpPost]
    public async Task<ActionResult<SubtaskResponse>> Create(int taskId,
        [FromBody] CreateSubtaskCommand createSubtaskCommand,
        CancellationToken cancellationToken)
    {
        createSubtaskCommand.TaskId = taskId;
        var subtask = await _mediator.Send(createSubtaskCommand, cancellationToken);

        return Created(nameof(GetById), subtask);
    }

    [HttpPut("{subtaskId:int}")]
    public async Task<ActionResult<IEnumerable<SubtaskResponse>>> Update(int subtaskId,
        [FromBody] UpdateSubtaskCommand updateSubtaskCommand,
        CancellationToken cancellationToken)
    {
        updateSubtaskCommand.SubtaskId = subtaskId;
        var subtask = await _mediator.Send(updateSubtaskCommand, cancellationToken);

        return Ok(subtask);
    }

    [HttpDelete("{subtaskId:int}")]
    public async Task<ActionResult<IEnumerable<Task>>> Delete(int subtaskId, CancellationToken cancellationToken)
    {
        var command = new DeleteSubtaskCommand { SubtaskId = subtaskId };
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}