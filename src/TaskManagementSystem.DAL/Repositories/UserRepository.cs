using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Repositories;

public class UserRepository : Repository<UserEntity>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }

    public async Task<bool> CheckIfActiveUserWithSameEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users.AnyAsync(x => x.Email == email && x.State == UserState.Active, cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}