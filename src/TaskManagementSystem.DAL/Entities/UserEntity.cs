using TaskManagementSystem.DAL.Abstractions;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.DAL.Entities;

public class UserEntity : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public UserState State { get; set; } = UserState.Active;
    public ICollection<TaskEntity> Tasks { get; set; } = [];
}