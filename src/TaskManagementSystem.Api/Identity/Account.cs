using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Api.Identity;

public class Account : IdentityUser
{
    public int UserId { get; set; }
}