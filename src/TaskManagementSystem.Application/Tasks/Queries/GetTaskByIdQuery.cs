using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Tasks.Models;

namespace TaskManagementSystem.Application.Tasks.Queries;

public class GetTaskByIdQuery : IRequest<TaskResponse?>
{
    public int TaskId { get; set; }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<TaskResponse?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.TaskRepository.GetByIdAsync(request.TaskId, cancellationToken);

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
}