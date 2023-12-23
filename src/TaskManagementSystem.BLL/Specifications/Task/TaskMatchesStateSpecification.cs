using System.Linq.Expressions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Specifications.Task;

public class TaskMatchesStateSpecification : ISpecification<DAL.Entities.TaskEntity>
{
    private readonly TaskState[] _states;

    public TaskMatchesStateSpecification(TaskState[] states)
    {
        _states = states;
    }

    public bool IsSatisfiedBy(DAL.Entities.TaskEntity entity)
    {
        return _states.Contains(entity.State);
    }

    public Expression<Func<DAL.Entities.TaskEntity, bool>> GetExpression()
    {
        return task => _states.Contains(task.State);
    }
}