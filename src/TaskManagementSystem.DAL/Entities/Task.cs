using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    public DateTime? DeadLine { get; set; }
    public string Category { get; set; }
    
    public ICollection<Subtask> Subtasks { get; set; } = [];
    
    public int UserId { get; set; }
    public User User { get; set; }
}