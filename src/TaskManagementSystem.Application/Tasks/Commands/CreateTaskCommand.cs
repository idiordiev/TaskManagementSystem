using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Subtasks.Commands;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Tasks.Commands;

public class CreateTaskCommand : IRequest<TaskResponse>
{
    public int UserId { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public DateTime? DeadLine { get; set; }
    
    public IEnumerable<CreateSubtaskCommand>? Subtasks { get; set; }
    
    [Required]
    public string Category { get; set; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAdmin && request.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }
        
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null || user.State == UserState.Deleted)
        {
            throw new NotFoundException("User", request.UserId);
        }

        var task = new TaskEntity
        {
            Name = request.Name,
            State = TaskState.Pending,
            DeadLine = request.DeadLine?.ToUniversalTime(),
            User = user,
            Subtasks = request.Subtasks?.Select(x => new SubtaskEntity
            {
                Name = x.Name,
                State = TaskState.Pending,
            }).ToList() ?? [],
            Category = request.Category
        };

        await _unitOfWork.TaskRepository.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TaskResponse>(task);
    }
}