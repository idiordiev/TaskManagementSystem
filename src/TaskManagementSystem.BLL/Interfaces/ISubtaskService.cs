using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ISubtaskService
{
    Task<IEnumerable<SubtaskResponse>> GetSubtasksForTaskAsync(int taskId, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> AddNewSubtaskAsync(int taskId, CreateSubtaskContract createSubtaskContract, CancellationToken cancellationToken = default);
    Task<SubtaskResponse> UpdateSubtask(int subtaskId, UpdateSubtaskContract updateSubtaskContract, CancellationToken cancellationToken = default);
    Task RemoveSubtaskAsync(int subtaskId, CancellationToken cancellationToken);
}