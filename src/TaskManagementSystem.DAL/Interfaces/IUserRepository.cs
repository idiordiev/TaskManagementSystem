using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<bool> CheckIfUserWithSameEmailExistsAsync(string email, CancellationToken cancellationToken = default);
}