using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Models;

public class TaskFiltersModel
{
    public string[] Categories { get; set; } = [];
    public TaskState[] States { get; set; } = [];
}