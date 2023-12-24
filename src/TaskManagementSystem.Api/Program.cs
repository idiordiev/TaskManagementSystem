using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskManagementSystem.Api.Middlewares;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Extensions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Infrastructure.Extensions;
using TaskManagementSystem.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddInfrastructure(configuration);
builder.Services.AddApplication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<JwtSettings>().BindConfiguration(JwtSettings.SectionName);
builder.Services.AddAuthentication(x =>
    {
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!)),
            ValidIssuer = configuration["JwtSettings:ValidIssuer"],
            ValidAudience = configuration["JwtSettings:ValidAudience"],

            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

await EnsureIdentityCreated(app.Services.CreateAsyncScope().ServiceProvider);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

async Task EnsureIdentityCreated(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (await roleManager.FindByNameAsync(IdentityRoleNames.User) is null)
    {
        await roleManager.CreateAsync(new IdentityRole(IdentityRoleNames.User));
    }
    
    if (await roleManager.FindByNameAsync(IdentityRoleNames.Admin) is null)
    {
        await roleManager.CreateAsync(new IdentityRole(IdentityRoleNames.Admin));
    }

    var userService = serviceProvider.GetRequiredService<IUserService>();
    var adminEmail = "admin@test.com";
    var user = await userService.GetByEmailAsync(adminEmail);

    if (user is null)
    {
        user = await userService.CreateAsync(new CreateUserContract
        {
            Name = "Admin",
            Email = adminEmail,
            Password = "Adm1nPasswordd"
        });
    }

    var identityService = serviceProvider.GetRequiredService<IIdentityService>();
    await identityService.TryGrantAdminRightsAsync(user.Id);
}

namespace TaskManagementSystem.Api
{
    public partial class Program;
}