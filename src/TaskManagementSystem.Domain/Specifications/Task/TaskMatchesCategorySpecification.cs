using Ardalis.Specification;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Domain.Specifications.Task;

public class TaskMatchesCategorySpecification : Specification<TaskEntity>
{
    public TaskMatchesCategorySpecification(string[] categories)
    {
        Query.Where(x => categories.Contains(x.Category));
    }
}