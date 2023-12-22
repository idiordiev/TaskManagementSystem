using System.Linq.Expressions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Enums;

namespace TaskManagementSystem.BLL.Specifications.Task;

public class TaskMatchesStateSpecification : ISpecification<DAL.Entities.Task>
{
    private readonly TaskState[] _states;

    public TaskMatchesStateSpecification(TaskState[] states)
    {
        _states = states;
    }

    public bool IsSatisfiedBy(DAL.Entities.Task entity)
    {
        return _states.Contains(entity.State);
    }

    public Expression<Func<DAL.Entities.Task, bool>> GetExpression()
    {
        return task => _states.Contains(task.State);
    }
}