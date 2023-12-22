using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Configurations;

public class SubtaskConfiguration : IEntityTypeConfiguration<Subtask>
{
    public void Configure(EntityTypeBuilder<Subtask> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Task)
            .WithMany(x => x.Subtasks)
            .HasForeignKey(x => x.TaskId);
    }
}