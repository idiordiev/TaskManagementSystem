using System.Linq.Expressions;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);

    Expression<Func<T, bool>> GetExpression();
}