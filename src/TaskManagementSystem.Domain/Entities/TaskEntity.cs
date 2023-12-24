using TaskManagementSystem.Domain.Abstractions;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Entities;

public class TaskEntity : Entity
{
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    public DateTime? DeadLine { get; set; }
    public string Category { get; set; }
    
    public ICollection<SubtaskEntity> Subtasks { get; set; } = [];
    
    public int UserId { get; set; }
    public UserEntity User { get; set; }
}