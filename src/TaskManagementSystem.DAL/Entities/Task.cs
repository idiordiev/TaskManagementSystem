using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    public ICollection<Task> Subtasks { get; set; } = new List<Task>();
    public ICollection<string> Categories { get; set; } = new List<string>();
}