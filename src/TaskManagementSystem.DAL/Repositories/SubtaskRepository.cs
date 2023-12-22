using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Repositories;

public class SubtaskRepository : Repository<Subtask>, ISubtaskRepository
{
    public SubtaskRepository(DataContext context) : base(context)
    {
    }
}