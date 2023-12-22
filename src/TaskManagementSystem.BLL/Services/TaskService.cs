using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Interfaces;
using TaskEntity = TaskManagementSystem.DAL.Entities.Task;

namespace TaskManagementSystem.BLL.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<TaskEntity>> GetAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TaskEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TaskEntity> AddAsync(CreateTaskContract newTask, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TaskEntity> UpdateAsync(int id, UpdateTaskContract task, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}