using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;
using TaskManagementSystem.Application.Models;

namespace TaskManagementSystem.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(int userId, TaskFiltersModel filters, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskResponse> AddAsync(int userId, CreateTaskContract newTask, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateAsync(int id, UpdateTaskContract taskToUpdate, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}