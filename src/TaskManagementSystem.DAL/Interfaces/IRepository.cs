using System.Linq.Expressions;
using TaskManagementSystem.DAL.Abstractions;

namespace TaskManagementSystem.DAL.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    void Delete(TEntity entity);
}