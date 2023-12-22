using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts;

public class UpdateTaskContract
{
    public string Name { get; set; }
    public TaskState State { get; set; }
    public DateTime? DeadLine { get; set; }
}