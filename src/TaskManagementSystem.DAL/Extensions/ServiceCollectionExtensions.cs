using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(x => x.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}