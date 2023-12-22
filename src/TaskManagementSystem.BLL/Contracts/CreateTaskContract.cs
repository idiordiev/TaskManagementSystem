namespace TaskManagementSystem.BLL.Contracts;

public class CreateTaskContract
{
    public string Name { get; set; }
    public DateTime? DeadLine { get; set; }
    public IEnumerable<CreateSubtaskContract> Subtasks { get; set; }
    public string Category { get; set; }
}