using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;

namespace TaskManagementSystem.Application.Subtasks.Commands;

public class DeleteSubtaskCommand : IRequest
{
    public int SubtaskId { get; set; }
}

public class DeleteSubtaskCommandHandler : IRequestHandler<DeleteSubtaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteSubtaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteSubtaskCommand request, CancellationToken cancellationToken)
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

        _unitOfWork.SubtaskRepository.Delete(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}