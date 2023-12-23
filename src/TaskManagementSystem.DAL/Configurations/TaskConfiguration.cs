using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.UserEntity)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.UserId);
    }
}