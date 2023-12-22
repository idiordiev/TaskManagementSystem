using TaskManagementSystem.DAL.Interfaces;
using Task = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.DAL.Repositories;

public class TaskRepository : Repository<Task>, ITaskRepository 
{
    public TaskRepository(DataContext context) : base(context)
    {
    }
}