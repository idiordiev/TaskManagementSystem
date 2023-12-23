using System.Linq.Expressions;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.BLL.Specifications.Task;

public class TaskMatchesCategorySpecification : ISpecification<DAL.Entities.TaskEntity>
{
    private readonly string[] _categories;

    public TaskMatchesCategorySpecification(string[] categories)
    {
        _categories = categories;
    }

    public bool IsSatisfiedBy(DAL.Entities.TaskEntity entity)
    {
        return _categories.Contains(entity.Category);
    }
    
    public Expression<Func<DAL.Entities.TaskEntity, bool>> GetExpression()
    {
        return task => _categories.Contains(task.Category);
    }
}