using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Infrastructure.Identity;

namespace TaskManagementSystem.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirstValue(IdentityClaims.UserIdClaim);

        if (user is not null && userIdClaim is not null)
        {
            UserId = int.Parse(userIdClaim);
            IsAdmin = user.HasClaim(IdentityClaims.IsAdminClaim, "true");
        }
    }

    public int UserId { get; } = -1;
    public bool IsAdmin { get; }
}