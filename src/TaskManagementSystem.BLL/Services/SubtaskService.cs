using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.Services;

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
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", taskId);
        }
        
        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Task", taskId);
        }

        return _mapper.Map<IEnumerable<SubtaskResponse>>(task.Subtasks);
    }

    public async Task<SubtaskResponse?> GetByIdAsync(int subtaskId, CancellationToken cancellationToken = default)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(subtaskId, cancellationToken);

        if (subtask is null)
        {
            return null;
        }
        
        if (!_currentUserService.IsAdmin && subtask.Task.UserId != _currentUserService.UserId)
        {
            return null;
        }
        
        return _mapper.Map<SubtaskResponse>(subtask);
    }

    public async Task<SubtaskResponse> AddToTaskAsync(int taskId, CreateSubtaskContract createSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", taskId);
        }
        
        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Task", taskId);
        }

        var newSubtask = new SubtaskEntity()
        {
            Name = createSubtaskContract.Name
        };
        
        task.Subtasks.Add(newSubtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(newSubtask);
    }

    public async Task<SubtaskResponse> UpdateAsync(int subtaskId, UpdateSubtaskContract updateSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(subtaskId, cancellationToken);
        
        if (subtask is null)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }
        
        if (!_currentUserService.IsAdmin && subtask.Task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }

        _mapper.Map(updateSubtaskContract, subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(subtask);
    }

    public async Task RemoveAsync(int subtaskId, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(subtaskId, cancellationToken);
        
        if (subtask is null)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }
        
        if (!_currentUserService.IsAdmin && subtask.Task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Subtask", subtaskId);
        }

        _unitOfWork.SubtaskRepository.Delete(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}