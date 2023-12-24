using System.Linq.Expressions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Specifications.Task;

public class TaskMatchesStateSpecification : ISpecification<TaskEntity>
{
    private readonly TaskState[] _states;

    public TaskMatchesStateSpecification(TaskState[] states)
    {
        _states = states;
    }

    public bool IsSatisfiedBy(TaskEntity entity)
    {
        return _states.Contains(entity.State);
    }

    public Expression<Func<TaskEntity, bool>> GetExpression()
    {
        return task => _states.Contains(task.State);
    }
}