﻿using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.BLL.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserContract createUserContract, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateAsync(int id, UpdateUserContract updateUserContract, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int userId, CancellationToken cancellationToken = default);
}