namespace TaskManagementSystem.Application.Interfaces;

public interface IUnitOfWork
{
    ITaskRepository TaskRepository { get; }
    ISubtaskRepository SubtaskRepository { get; }
    IUserRepository UserRepository { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void SaveChanges();
}