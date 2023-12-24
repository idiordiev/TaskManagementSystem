using System.Linq.Expressions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Specifications.Task;

public class TaskMatchesCategorySpecification : ISpecification<TaskEntity>
{
    private readonly string[] _categories;

    public TaskMatchesCategorySpecification(string[] categories)
    {
        _categories = categories;
    }

    public bool IsSatisfiedBy(TaskEntity entity)
    {
        return _categories.Contains(entity.Category);
    }

    public Expression<Func<TaskEntity, bool>> GetExpression()
    {
        return task => _categories.Contains(task.Category);
    }
}