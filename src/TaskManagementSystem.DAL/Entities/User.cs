using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserState State { get; set; } = UserState.Active;
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}