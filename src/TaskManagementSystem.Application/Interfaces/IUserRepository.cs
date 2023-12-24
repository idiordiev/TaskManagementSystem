using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<bool> CheckIfActiveUserWithSameEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}