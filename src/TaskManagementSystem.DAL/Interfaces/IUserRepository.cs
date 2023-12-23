using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Interfaces;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<bool> CheckIfActiveUserWithSameEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}