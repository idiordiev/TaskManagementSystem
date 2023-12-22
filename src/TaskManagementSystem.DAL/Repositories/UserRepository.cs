using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.DAL.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context)
    {
    }
}