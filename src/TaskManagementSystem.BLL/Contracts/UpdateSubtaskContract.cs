using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts;

public class UpdateSubtaskContract
{
    public string Name { get; set; }
    public TaskState State { get; set; }
}