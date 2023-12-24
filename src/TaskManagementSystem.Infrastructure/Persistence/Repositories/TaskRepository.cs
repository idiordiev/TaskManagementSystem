using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories;

public class TaskRepository : Repository<TaskEntity>, ITaskRepository
{
    public TaskRepository(DataContext context) : base(context)
    {
    }

    public new async Task<IEnumerable<TaskEntity>> GetAsync(Expression<Func<TaskEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = Context.Tasks
            .Include(x => x.Subtasks);

        if (predicate is null)
        {
            return await dbSet.ToListAsync(cancellationToken);
        }

        return await dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public new async Task<TaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Tasks
            .Include(x => x.Subtasks)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}