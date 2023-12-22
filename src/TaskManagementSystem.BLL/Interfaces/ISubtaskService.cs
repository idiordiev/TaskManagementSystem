using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.DAL.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ISubtaskService
{
    Task<IEnumerable<Subtask>> GetSubtasksForTaskAsync(int taskId, CancellationToken cancellationToken = default);
    Task<Subtask> AddNewSubtaskAsync(int taskId, CreateSubtaskContract createSubtaskContract, CancellationToken cancellationToken = default);
    Task<Subtask> UpdateSubtask(int subtaskId, UpdateSubtaskContract updateSubtaskContract, CancellationToken cancellationToken = default);
    Task RemoveSubtaskAsync(int subtaskId, CancellationToken cancellationToken);
}