using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Subtasks.Models;

namespace TaskManagementSystem.Application.Subtasks.Queries;

public class GetSubtasksByTaskIdQuery : IRequest<IEnumerable<SubtaskResponse>>
{
    public int TaskId { get; set; }
}

public class GetSubtasksByTaskIdQueryHandler : IRequestHandler<GetSubtasksByTaskIdQuery, IEnumerable<SubtaskResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetSubtasksByTaskIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<SubtaskResponse>> Handle(GetSubtasksByTaskIdQuery request, CancellationToken cancellationToken)
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
        
        return _mapper.Map<IEnumerable<SubtaskResponse>>(task.Subtasks);
    }
}