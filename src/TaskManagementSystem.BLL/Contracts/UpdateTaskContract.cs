using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts;

public class UpdateTaskContract
{
    [Required]
    public string Name { get; set; }
    
    public TaskState State { get; set; } = TaskState.Pending;
    
    public DateTime? DeadLine { get; set; }
}