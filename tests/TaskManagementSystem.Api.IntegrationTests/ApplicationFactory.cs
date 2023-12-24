using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagementSystem.Api.Identity;
using TaskManagementSystem.DAL;

namespace TaskManagementSystem.Api.IntegrationTests;

public class ApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<DataContext>>();
            services.RemoveAll<DataContext>();

            services.RemoveAll<DbContextOptions<IdentityContext>>();
            services.RemoveAll<IdentityContext>();

            services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
            services.AddDbContext<IdentityContext>(x => x.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
        });
    }
}