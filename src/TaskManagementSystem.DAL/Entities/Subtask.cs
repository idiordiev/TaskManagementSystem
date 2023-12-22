using TaskManagementSystem.DAL.Abstractions;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class Subtask : Entity
{
    public string Name { get; set; }
    public TaskState State { get; set; } = TaskState.Pending;
    
    public int TaskId { get; set; }
    public Task Task { get; set; }
}