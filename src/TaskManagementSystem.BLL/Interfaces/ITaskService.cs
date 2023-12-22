using TaskManagementSystem.BLL.Contracts;
using TaskEntity = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.BLL.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskEntity>> GetAsync(CancellationToken cancellationToken = default);
    Task<TaskEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskEntity> AddAsync(CreateTaskContract newTask, CancellationToken cancellationToken = default);
    Task<TaskEntity> UpdateAsync(int id, UpdateTaskContract task, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
}