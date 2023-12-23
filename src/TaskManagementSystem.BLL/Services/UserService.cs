using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
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
    private readonly IMapper _mapper;

    public UserService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.UserRepository.GetAsync(x => x.State != UserState.Deleted, cancellationToken);

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(email, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserContract createUserContract, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(createUserContract.Email,
                cancellationToken))
        {
            throw new UserExistsException($"User with email {createUserContract.Email} already exists");
        }
        
        var user = new UserEntity
        {
            Name = createUserContract.Name,
            Email = createUserContract.Email,
        };

        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _identityService.CreateAccountAsync(createUserContract.Email, createUserContract.Password, user.Id);
        }
        catch (Exception)
        {
            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserContract updateUserContract, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", id);
        }

        _mapper.Map(updateUserContract, user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeactivateUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }
        
        await _identityService.DeleteAccountsForUserAsync(user.Id, cancellationToken);
        
        user.State = UserState.Deleted;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}