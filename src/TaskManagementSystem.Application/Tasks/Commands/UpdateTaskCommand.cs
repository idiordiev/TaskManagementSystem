using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Tasks.Commands;

public class UpdateTaskCommand : IRequest<TaskResponse>
{
    public int TaskId { get; set; }

    [Required]
    public string Name { get; set; }

    public TaskState State { get; set; } = TaskState.Pending;

    public DateTime? DeadLine { get; set; }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(request.TaskId, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task", request.TaskId);
        }

        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        task.Name = request.Name;
        task.State = request.State;
        task.DeadLine = request.DeadLine?.ToUniversalTime();

        _unitOfWork.TaskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskResponse>(task);
    }
}