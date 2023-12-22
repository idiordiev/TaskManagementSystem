using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Exceptions;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Enums;
using TaskManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public UserService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task CreateUserAsync(CreateUserContract createUserContract, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.UserRepository.CheckIfUserWithSameEmailExistsAsync(createUserContract.Email,
                cancellationToken))
        {
            throw new UserExistsException($"User with email {createUserContract.Email} already exists");
        }
        
        var user = new User
        {
            Name = createUserContract.Name,
            Email = createUserContract.Email,
        };

        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityService.CreateAccountAsync(createUserContract.Email, createUserContract.Password, user.Id);
    }

    public async Task DeactivateUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }
        
        user.State = UserState.Deleted;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _identityService.DeleteAccountsForUserAsync(user.Id, cancellationToken);
    }
}