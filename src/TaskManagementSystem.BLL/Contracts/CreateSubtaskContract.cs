using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.BLL.Contracts;

public class CreateSubtaskContract
{
    [Required]
    public string Name { get; set; }
}