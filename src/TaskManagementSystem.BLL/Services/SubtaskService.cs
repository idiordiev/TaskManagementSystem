using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManagementSystem.BLL.Services;

public class SubtaskService : ISubtaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubtaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Task<IEnumerable<Subtask>> GetSubtasksForTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Subtask> AddNewSubtaskAsync(int taskId, CreateSubtaskContract createSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Subtask> UpdateSubtask(int subtaskId, UpdateSubtaskContract updateSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveSubtaskAsync(int subtaskId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}