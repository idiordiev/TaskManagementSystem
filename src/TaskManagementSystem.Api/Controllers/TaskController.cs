using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Models;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Task>>> GetAll(int userId, [FromQuery] TaskFiltersModel filters,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskService.GetTasksForUserAsync(userId, filters, cancellationToken);

        return Ok(tasks);
    }
    
    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<IEnumerable<Task>>> GetById(int taskId, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetByIdAsync(taskId, cancellationToken);

        return Ok(task);
    }
    
    [HttpPost]
    public async Task<ActionResult<Task>> Create(int userId, [FromBody] CreateTaskContract createTaskContract,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.AddAsync(userId, createTaskContract, cancellationToken);

        return Created(nameof(GetById), task);
    }
    
    [HttpPut("{taskId:int}")]
    public async Task<ActionResult<IEnumerable<Task>>> Update(int taskId, [FromBody] UpdateTaskContract updateTaskContract,
        CancellationToken cancellationToken)
    {
        var task = await _taskService.UpdateAsync(taskId, updateTaskContract, cancellationToken);

        return Ok(task);
    }
    
    [HttpDelete("{taskId:int}")]
    public async Task<ActionResult<IEnumerable<Task>>> Delete(int taskId, CancellationToken cancellationToken)
    {
        await _taskService.DeleteByIdAsync(taskId, cancellationToken);

        return Ok();
    }
}