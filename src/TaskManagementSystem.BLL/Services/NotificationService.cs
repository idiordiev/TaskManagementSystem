using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Models;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly TimeProvider _timeProvider;

    public NotificationService(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.IsAdmin && userId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        var now = _timeProvider.GetUtcNow();

        var tasks = await _unitOfWork.TaskRepository.GetAsync(x => x.UserId == userId 
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