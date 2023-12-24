using AutoMapper;
using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.Application.Services;

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

    public async Task<UserResponse> CreateAsync(CreateUserContract createUserContract, CancellationToken cancellationToken = default)
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

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserContract updateUserContract, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", id);
        }

        _mapper.Map(updateUserContract, user);
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeactivateAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }
        
        await _identityService.DeleteAccountsForUserAsync(user.Id, cancellationToken);
        
        user.State = UserState.Deleted;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}