﻿using System.Linq.Expressions;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.BLL.Specifications.Task;

public class TaskBelongsToUserSpecification : ISpecification<DAL.Entities.Task>
{
    private readonly int _userId;

    public TaskBelongsToUserSpecification(int userId)
    {
        _userId = userId;
    }

    public bool IsSatisfiedBy(DAL.Entities.Task entity)
    {
        return entity.UserId == _userId;
    }
    
    public Expression<Func<DAL.Entities.Task, bool>> GetExpression()
    {
        return task => task.UserId == _userId;
    }
}