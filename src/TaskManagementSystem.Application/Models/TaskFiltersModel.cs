using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Models;

public class TaskFiltersModel
{
    public string[] Categories { get; set; } = [];
    public TaskState[] States { get; set; } = [];
}