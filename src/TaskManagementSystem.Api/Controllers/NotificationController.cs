using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Models;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetAll(int userId, CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetNotificationsAsync(userId, cancellationToken);

        return Ok(notifications);
    }
}