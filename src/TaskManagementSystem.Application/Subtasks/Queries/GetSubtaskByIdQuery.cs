using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Subtasks.Models;

namespace TaskManagementSystem.Application.Subtasks.Queries;

public class GetSubtaskByIdQuery : IRequest<SubtaskResponse?>
{
    public int SubtaskId { get; set; }
}

public class GetSubtaskByIdQueryHandler : IRequestHandler<GetSubtaskByIdQuery, SubtaskResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetSubtaskByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<SubtaskResponse?> Handle(GetSubtaskByIdQuery request, CancellationToken cancellationToken)
    {
        var subtask = await _unitOfWork.SubtaskRepository.GetByIdAsync(request.SubtaskId, cancellationToken);

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
}