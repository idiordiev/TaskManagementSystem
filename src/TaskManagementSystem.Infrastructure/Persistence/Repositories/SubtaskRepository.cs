using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories;

public class SubtaskRepository : Repository<SubtaskEntity>, ISubtaskRepository
{
    public SubtaskRepository(DataContext context) : base(context)
    {
    }

    public new async Task<IEnumerable<SubtaskEntity>> GetAsync(Expression<Func<SubtaskEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = Context.Subtasks
            .Include(x => x.Task);

        if (predicate is null)
        {
            return await dbSet.ToListAsync(cancellationToken);
        }

        return await dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public new async Task<SubtaskEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Subtasks
            .Include(x => x.Task)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}