using Ardalis.Specification;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Domain.Specifications.Task;

public class TaskMatchesStateSpecification : Specification<TaskEntity>
{
    public TaskMatchesStateSpecification(TaskState[] states)
    {
        Query.Where(x => states.Contains(x.State));
    }
}