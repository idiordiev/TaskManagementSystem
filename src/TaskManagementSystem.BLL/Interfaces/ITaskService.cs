using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.BLL.Models;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetTasksForUserAsync(int userId, TaskFiltersModel filters, CancellationToken cancellationToken = default);
    Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskResponse> AddAsync(int userId, CreateTaskContract newTask, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateAsync(int id, UpdateTaskContract taskToUpdate, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}