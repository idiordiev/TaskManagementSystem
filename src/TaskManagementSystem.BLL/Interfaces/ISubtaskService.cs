using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ISubtaskService
{
    Task<IEnumerable<SubtaskResponse>> GetSubtasksByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<SubtaskResponse?> GetByIdAsync(int subtaskId, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> AddToTaskAsync(int taskId, CreateSubtaskContract createSubtaskContract, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> UpdateAsync(int subtaskId, UpdateSubtaskContract updateSubtaskContract, CancellationToken cancellationToken = default);
    Task RemoveAsync(int subtaskId, CancellationToken cancellationToken = default);
}