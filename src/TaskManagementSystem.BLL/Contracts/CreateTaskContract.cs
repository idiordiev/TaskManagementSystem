using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.BLL.Contracts;

public class CreateTaskContract
{
    [Required]
    public string Name { get; set; }
    
    public DateTime? DeadLine { get; set; }
    
    public IEnumerable<CreateSubtaskContract>? Subtasks { get; set; }
    [Required]
    
    public string Category { get; set; }
}