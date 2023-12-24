using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Infrastructure.Persistence.Configurations;

public class SubtaskConfiguration : IEntityTypeConfiguration<SubtaskEntity>
{
    public void Configure(EntityTypeBuilder<SubtaskEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Task)
            .WithMany(x => x.Subtasks)
            .HasForeignKey(x => x.TaskId);
    }
}