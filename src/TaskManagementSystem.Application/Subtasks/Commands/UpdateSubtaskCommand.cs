using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Subtasks.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Subtasks.Commands;

public class UpdateSubtaskCommand : IRequest<SubtaskResponse>
{
    public int SubtaskId { get; set; }

    [Required]
    public string Name { get; set; }

    public TaskState State { get; set; }
}

public class UpdateSubtaskCommandHandler : IRequestHandler<UpdateSubtaskCommand, SubtaskResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateSubtaskCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<SubtaskResponse> Handle(UpdateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(request.SubtaskId, cancellationToken);

        if (subtask is null)
        {
            throw new NotFoundException("Subtask", request.SubtaskId);
        }

        if (!_currentUserService.IsAdmin && subtask.Task.UserId != _currentUserService.UserId)
        {
            throw new NotFoundException("Subtask", request.SubtaskId);
        }

        subtask.Name = request.Name;
        subtask.State = request.State;

        _unitOfWork.SubtaskRepository.Update(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubtaskResponse>(subtask);
    }
}