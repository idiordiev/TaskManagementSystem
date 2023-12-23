using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Contracts.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserState State { get; set; } = UserState.Active;
}