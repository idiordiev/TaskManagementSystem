using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.BLL.Interfaces;

namespace TaskManagementSystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("IdentityConnection"));
        });
        
        services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
    }
}