using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Application.Contracts;

public class CreateSubtaskContract
{
    [Required]
    public string Name { get; set; }
}