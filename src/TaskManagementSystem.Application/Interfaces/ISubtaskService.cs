using TaskManagementSystem.Application.Contracts;
using TaskManagementSystem.Application.Contracts.Responses;

namespace TaskManagementSystem.Application.Interfaces;

public interface ISubtaskService
{
    Task<IEnumerable<SubtaskResponse>> GetSubtasksByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<SubtaskResponse?> GetByIdAsync(int subtaskId, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> AddToTaskAsync(int taskId, CreateSubtaskContract createSubtaskContract, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> UpdateAsync(int subtaskId, UpdateSubtaskContract updateSubtaskContract, CancellationToken cancellationToken = default);
    Task DeleteAsync(int subtaskId, CancellationToken cancellationToken = default);
}