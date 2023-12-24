using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Infrastructure.Identity;

public class Account : IdentityUser
{
    public int UserId { get; set; }
}