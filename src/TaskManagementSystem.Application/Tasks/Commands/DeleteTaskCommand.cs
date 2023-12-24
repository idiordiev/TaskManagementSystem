using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;

namespace TaskManagementSystem.Application.Tasks.Commands;

public class DeleteTaskCommand : IRequest
{
    public int TaskId { get; set; }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;


    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
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

        _unitOfWork.TaskRepository.Delete(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}