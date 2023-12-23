using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/tasks/{taskId:int}/subtasks")]
[Authorize]
public class SubtaskController : ControllerBase
{
    private readonly ISubtaskService _subtaskService;

    public SubtaskController(ISubtaskService subtaskService)
    {
        _subtaskService = subtaskService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubtaskResponse>>> GetByTaskId(int taskId, CancellationToken cancellationToken)
    {
        var subtasks = await _subtaskService.GetSubtasksByTaskIdAsync(taskId, cancellationToken);

        return Ok(subtasks);
    }
    
    [HttpGet("{subtaskId:int}")]
    public async Task<ActionResult<SubtaskResponse>> GetById(int subtaskId, CancellationToken cancellationToken)
    {
        var subtask = await _subtaskService.GetByIdAsync(subtaskId, cancellationToken);

        return Ok(subtask);
    }
    
    [HttpPost]
    public async Task<ActionResult<SubtaskResponse>> Create(int taskId, [FromBody] CreateSubtaskContract createSubtaskContract,
        CancellationToken cancellationToken)
    {
        var subtask = await _subtaskService.AddToTaskAsync(taskId, createSubtaskContract, cancellationToken);

        return Created(nameof(GetById), subtask);
    }
    
    [HttpPut("{subtaskId:int}")]
    public async Task<ActionResult<IEnumerable<SubtaskResponse>>> Update(int subtaskId, [FromBody] UpdateSubtaskContract updateSubtaskContract,
        CancellationToken cancellationToken)
    {
        var subtask = await _subtaskService.UpdateAsync(subtaskId, updateSubtaskContract, cancellationToken);

        return Ok(subtask);
    }
    
    [HttpDelete("{subtaskId:int}")]
    public async Task<ActionResult<IEnumerable<Task>>> Delete(int subtaskId, CancellationToken cancellationToken)
    {
        await _subtaskService.RemoveAsync(subtaskId, cancellationToken);

        return Ok();
    }
}