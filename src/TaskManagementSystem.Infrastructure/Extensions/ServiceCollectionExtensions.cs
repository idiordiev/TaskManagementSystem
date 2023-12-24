using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Infrastructure.Identity;
using TaskManagementSystem.Infrastructure.Persistence;

namespace TaskManagementSystem.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureIdentity(configuration);
        services.AddDbContext<DataContext>(x => x.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
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