using System.Linq.Expressions;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.BLL.Specifications.Task;

public class TaskMatchesCategorySpecification : ISpecification<DAL.Entities.Task>
{
    private readonly string[] _categories;

    public TaskMatchesCategorySpecification(string[] categories)
    {
        _categories = categories;
    }

    public bool IsSatisfiedBy(DAL.Entities.Task entity)
    {
        return _categories.Contains(entity.Category);
    }
    
    public Expression<Func<DAL.Entities.Task, bool>> GetExpression()
    {
        return task => _categories.Contains(task.Category);
    }
}