using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Subtasks.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Subtasks.Commands;

public class CreateSubtaskCommand : IRequest<SubtaskResponse>
{
    public int TaskId { get; set; }
    
    [Required]
    public string Name { get; set; }
}

public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, SubtaskResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateSubtaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<SubtaskResponse> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task is null)
        {
            throw new NotFoundException("Task", request.TaskId);
        }
        
        if (!_currentUserService.IsAdmin && task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Task", request.TaskId);
        }

        var newSubtask = new SubtaskEntity
        {
            Name = request.Name,
            State = TaskState.Pending,
            Task = task
        };

        await _unitOfWork.SubtaskRepository.AddAsync(newSubtask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(newSubtask);
    }
}