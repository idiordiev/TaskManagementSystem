using TaskManagementSystem.Application.Subtasks.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Tasks.Models;

public class TaskResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }
    public TaskState State { get; set; }
    public DateTime? DeadLine { get; set; }
    public string Category { get; set; }
    public IEnumerable<SubtaskResponse> Subtasks { get; set; }
}