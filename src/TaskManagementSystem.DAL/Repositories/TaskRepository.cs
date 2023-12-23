using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Repositories;

public class TaskRepository : Repository<TaskEntity>, ITaskRepository 
{
    public TaskRepository(DataContext context) : base(context)
    {
    }
}