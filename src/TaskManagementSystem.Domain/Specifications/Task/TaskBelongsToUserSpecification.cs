using Ardalis.Specification;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Specifications.Task;

public class TaskBelongsToUserSpecification : Specification<TaskEntity>
{
    public TaskBelongsToUserSpecification(int userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}