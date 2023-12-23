using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts;

public class UpdateSubtaskContract
{
    [Required]
    public string Name { get; set; }

    public TaskState State { get; set; }
}