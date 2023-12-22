using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Entities;
using Task = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.DAL;

public class DataContext : DbContext
{
    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Subtask> Subtasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}