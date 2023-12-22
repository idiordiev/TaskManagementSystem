using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }

    public async Task<bool> CheckIfUserWithSameEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Users.AnyAsync(x => x.Email == email, cancellationToken);
    }
}