using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts.Responses;

public class SubtaskResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskState State { get; set; }
}