namespace TaskManagementSystem.Application.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAdmin { get; }
}