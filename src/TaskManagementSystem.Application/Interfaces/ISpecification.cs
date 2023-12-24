using System.Linq.Expressions;

namespace TaskManagementSystem.Application.Interfaces;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);

    Expression<Func<T, bool>> GetExpression();
}