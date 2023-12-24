using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Application.Notifications.Models;
using TaskManagementSystem.Application.Notifications.Queries;

namespace TaskManagementSystem.Api.Controllers;

[ApiController]
[Route("api/users/{userId:int}/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetAll(int userId, CancellationToken cancellationToken)
    {
        var query = new GetNotificationsQuery { UserId = userId };
        var notifications = await _mediator.Send(query, cancellationToken);

        return Ok(notifications);
    }
}