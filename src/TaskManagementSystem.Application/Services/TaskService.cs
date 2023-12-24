using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Specifications.Task;
using TaskManagementSystem.Application.Utility;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public TaskService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(int userId, TaskFiltersModel filters, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAdmin && userId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }
        
        var specs = new List<ISpecification<TaskEntity>>();

        specs.Add(new TaskBelongsToUserSpecification(_currentUserService.UserId));
        
        if (filters.Categories.Length != 0)
        {
            specs.Add(new TaskMatchesCategorySpecification(filters.Categories));
        }

        if (filters.States.Length != 0)
        {
            specs.Add(new TaskMatchesStateSpecification(filters.States));
        }

        var predicate = PredicateBuilder.True<TaskEntity>();

        foreach (var spec in specs)
        {
            predicate = predicate.And(spec.GetExpression());
        }

        var tasks = await _unitOfWork.TaskRepository.GetAsync(predicate, cancellationToken);

        return _mapper.Map<IEnumerable<TaskResponse>>(tasks);
    }

    public async Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(id, cancellationToken);

        if (task is null)
        {
            return _mapper.Map<TaskResponse>(task);
        }

        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            return null;
        }

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> AddAsync(int userId, CreateTaskContract newTask, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAdmin && userId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }
        
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null || user.State == UserState.Deleted)
        {
            throw new NotFoundException("User", userId);
        }

        var task = new TaskEntity
        {
            Name = newTask.Name,
            State = TaskState.Pending,
            DeadLine = newTask.DeadLine?.ToUniversalTime(),
            User = user,
            Subtasks = newTask.Subtasks.Select(x => new SubtaskEntity
            {
                Name = x.Name,
                State = TaskState.Pending,
            }).ToList(),
            Category = newTask.Category
        };

        await _unitOfWork.TaskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task<TaskResponse> UpdateAsync(int id, UpdateTaskContract taskToUpdate, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }
        
        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        _mapper.Map(taskToUpdate, task);

        _unitOfWork.TaskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskResponse>(task);
    }

    public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", id);
        }

        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        _unitOfWork.TaskRepository.Delete(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}