using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Api.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public int UserId { get; set; }
}