using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Infrastructure.Identity;

public class IdentityContext : IdentityDbContext<Account>
{
    public IdentityContext()
    {
    }
    
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
}