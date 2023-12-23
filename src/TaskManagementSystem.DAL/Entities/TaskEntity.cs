using TaskManagementSystem.DAL.Abstractions;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class TaskEntity : Entity
{
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    public DateTime? DeadLine { get; set; }
    public string Category { get; set; }
    
    public ICollection<SubtaskEntity> Subtasks { get; set; } = [];
    
    public int UserId { get; set; }
    public UserEntity UserEntity { get; set; }
}