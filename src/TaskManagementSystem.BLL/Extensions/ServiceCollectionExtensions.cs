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
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}