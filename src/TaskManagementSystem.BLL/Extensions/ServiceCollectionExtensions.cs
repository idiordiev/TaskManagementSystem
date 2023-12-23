using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.BLL.Services;

namespace TaskManagementSystem.BLL.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ISubtaskService, SubtaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<TimeProvider>(_ => TimeProvider.System);
        services.AddSingleton<TimeProvider>(_ => TimeProvider.System);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}