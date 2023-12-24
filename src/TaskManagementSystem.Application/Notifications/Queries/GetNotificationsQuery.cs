using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Models;
using TaskManagementSystem.Application.Notifications.Models;

namespace TaskManagementSystem.Application.Notifications.Queries;

public class GetNotificationsQuery : IRequest<IEnumerable<Notification>>
{
    public int UserId { get; set; }
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IEnumerable<Notification>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async Task<IEnumerable<Notification>> Handle(GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAdmin && request.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        var now = _timeProvider.GetUtcNow();

        var tasks = await _unitOfWork.TaskRepository.GetAsync(x => x.UserId == request.UserId
                                                                   && x.DeadLine >= now
                                                                   && x.DeadLine <= now.AddDays(1),
            cancellationToken);
        var notifications = new List<Notification>();

        foreach (var task in tasks)
        {
            var expiresIn = task.DeadLine!.Value - now;

            notifications.Add(new Notification
            {
                UserId = task.UserId,
                TaskId = task.Id,
                ExpiresIn = expiresIn,
                Message = $"This task expires in {expiresIn}"
            });
        }

        return notifications;
    }
}