using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Contracts;

public class UpdateSubtaskContract
{
    [Required]
    public string Name { get; set; }

    public TaskState State { get; set; }
}