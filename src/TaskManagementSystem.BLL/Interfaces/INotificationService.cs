using TaskManagementSystem.BLL.Models;

namespace TaskManagementSystem.BLL.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetNotificationsAsync(int userId, CancellationToken cancellationToken = default);
}