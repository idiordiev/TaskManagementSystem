using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user is not null)
        {
            // UserId = int.Parse(user.FindFirstValue(IdentityClaims.UserIdClaim));
            // IsAdmin = user.HasClaim(IdentityClaims.IsAdminClaim, "true");
        }
    }

    public int UserId { get; } = -1;
    public bool IsAdmin { get; }
}