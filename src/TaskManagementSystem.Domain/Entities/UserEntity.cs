using TaskManagementSystem.Domain.Abstractions;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Entities;

public class UserEntity : Entity
{
    public string Name { get; set; }
    public string Email { get; set; }
    public UserState State { get; set; } = UserState.Active;
    public ICollection<TaskEntity> Tasks { get; set; } = [];
}