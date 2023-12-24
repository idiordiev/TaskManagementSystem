using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Services;

public class SubtaskService : ISubtaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SubtaskService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<SubtaskResponse>> GetSubtasksByTaskIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await GetTaskByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", taskId);
        }

        return _mapper.Map<IEnumerable<SubtaskResponse>>(task.Subtasks);
    }

    private async Task<TaskEntity?> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            return null;
        }
        
        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            return null;
        }

        return task;
    }

    public async Task<SubtaskResponse?> GetByIdAsync(int subtaskId, CancellationToken cancellationToken = default)
    {
        var subtask = await GetSubtaskByIdAsync(subtaskId, cancellationToken);

        return _mapper.Map<SubtaskResponse>(subtask);
    }

    private async Task<SubtaskEntity?> GetSubtaskByIdAsync(int subtaskId, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(subtaskId, cancellationToken);

        if (subtask is null)
        {
            return subtask;
        }
        
        if (!_currentUserService.IsAdmin && subtask.Task.UserId != _currentUserService.UserId)
        {
            return null;
        }

        return subtask;
    }

    public async Task<SubtaskResponse> AddToTaskAsync(int taskId, CreateSubtaskContract createSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        var task = await GetTaskByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", taskId);
        }

        var newSubtask = new SubtaskEntity
        {
            Name = createSubtaskContract.Name,
            State = TaskState.Pending,
            Task = task
        };

        await _unitOfWork.SubtaskRepository.AddAsync(newSubtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(newSubtask);
    }

    public async Task<SubtaskResponse> UpdateAsync(int subtaskId, UpdateSubtaskContract updateSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        var subtask = await GetSubtaskByIdAsync(subtaskId, cancellationToken);
        
        if (subtask is null)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }
        
        _mapper.Map(updateSubtaskContract, subtask);
        _unitOfWork.SubtaskRepository.Update(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(subtask);
    }

    public async Task DeleteAsync(int subtaskId, CancellationToken cancellationToken = default)
    {
        var subtask =  await GetSubtaskByIdAsync(subtaskId, cancellationToken);
        
        if (subtask is null)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }

        _unitOfWork.SubtaskRepository.Delete(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}