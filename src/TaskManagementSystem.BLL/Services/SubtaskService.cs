using AutoMapper;
using TaskManagementSystem.BLL.Contracts;
using TaskManagementSystem.BLL.Contracts.Responses;
using TaskManagementSystem.BLL.Interfaces;
using TaskManagementSystem.DAL.Interfaces;

namespace TaskManagementSystem.BLL.Services;

public class SubtaskService : ISubtaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubtaskService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public Task<IEnumerable<SubtaskResponse>> GetSubtasksForTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SubtaskResponse> AddNewSubtaskAsync(int taskId, CreateSubtaskContract createSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SubtaskResponse> UpdateSubtask(int subtaskId, UpdateSubtaskContract updateSubtaskContract,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveSubtaskAsync(int subtaskId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}