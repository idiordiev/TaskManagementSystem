using Task = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.DAL.Interfaces;

public interface ITaskRepository : IRepository<Task>
{
}