using System.Linq.Expressions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Specifications.Task;

public class TaskBelongsToUserSpecification : ISpecification<TaskEntity>
{
    private readonly int _userId;

    public TaskBelongsToUserSpecification(int userId)
    {
        _userId = userId;
    }

    public bool IsSatisfiedBy(TaskEntity entity)
    {
        return entity.UserId == _userId;
    }
    
    public Expression<Func<TaskEntity, bool>> GetExpression()
    {
        return task => task.UserId == _userId;
    }
}