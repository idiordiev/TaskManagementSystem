using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts.Responses;

public class TaskResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskState State { get; set; }
    public DateTime? DeadLine { get; set; }
    public string Category { get; set; }
    public IEnumerable<SubtaskResponse> Subtasks { get; set; } 
}