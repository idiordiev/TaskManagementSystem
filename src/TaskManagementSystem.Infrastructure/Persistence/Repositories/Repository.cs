using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Abstractions;

namespace TaskManagementSystem.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly DataContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected Repository(DataContext context)
    {
        Context = context;
        DbSet = Context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        if (predicate is not null)
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        return await DbSet.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            return;
        }

        DbSet.Remove(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}