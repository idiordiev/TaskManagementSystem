using TaskManagementSystem.Application.Models;

namespace TaskManagementSystem.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default);
}