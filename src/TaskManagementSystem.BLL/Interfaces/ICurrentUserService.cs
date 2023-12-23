namespace TaskManagementSystem.BLL.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAdmin { get; }
}