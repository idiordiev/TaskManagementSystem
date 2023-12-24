using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Contracts.Responses;

public class SubtaskResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TaskId { get; set; }
    public TaskState State { get; set; }
}