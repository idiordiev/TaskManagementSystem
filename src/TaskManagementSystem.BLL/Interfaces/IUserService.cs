using TaskManagementSystem.BLL.Contracts;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.BLL.Interfaces;

public interface IUserService
{
    Task CreateUserAsync(CreateUserContract createUserContract, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(int userId, CancellationToken cancellationToken = default);
}