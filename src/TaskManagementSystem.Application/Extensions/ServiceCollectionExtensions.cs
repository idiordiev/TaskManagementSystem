using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagementSystem.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<TimeProvider>(_ => TimeProvider.System);
        services.AddSingleton<TimeProvider>(_ => TimeProvider.System);

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}