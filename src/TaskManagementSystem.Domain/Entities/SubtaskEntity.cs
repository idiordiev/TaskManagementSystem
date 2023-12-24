using TaskManagementSystem.Domain.Abstractions;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Entities;

public class SubtaskEntity : Entity
{
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    
    public int TaskId { get; set; }
    public TaskEntity Task { get; set; }
}