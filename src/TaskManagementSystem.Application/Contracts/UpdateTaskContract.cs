using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Contracts;

public class UpdateTaskContract
{
    [Required]
    public string Name { get; set; }
    
    public TaskState State { get; set; } = TaskState.Pending;
    
    public DateTime? DeadLine { get; set; }
}