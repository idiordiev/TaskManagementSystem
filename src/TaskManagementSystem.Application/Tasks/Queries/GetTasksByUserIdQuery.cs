using Ardalis.Specification;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Tasks.Models;
using TaskManagementSystem.Application.Utility;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Specifications.Task;

namespace TaskManagementSystem.Application.Tasks.Queries;

public class GetTasksByUserIdQuery : IRequest<IEnumerable<TaskResponse>>
{
    public int UserId { get; set; }
    public TaskFiltersModel Filters { get; set; } = new TaskFiltersModel();
}

public class GetTasksByUserIdQueryHandler : IRequestHandler<GetTasksByUserIdQuery, IEnumerable<TaskResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public GetTasksByUserIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TaskResponse>> Handle(GetTasksByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAdmin && request.UserId != _currentUserService.UserId)
        {
            throw new ForbiddenException();
        }

        var specs = new List<Specification<TaskEntity>>();

        specs.Add(new TaskBelongsToUserSpecification(_currentUserService.UserId));

        if (request.Filters.Categories.Length != 0)
        {
            specs.Add(new TaskMatchesCategorySpecification(request.Filters.Categories));
        }

        if (request.Filters.States.Length != 0)
        {
            specs.Add(new TaskMatchesStateSpecification(request.Filters.States));
        }

        var predicate = PredicateBuilder.True<TaskEntity>();

        foreach (var spec in specs)
        {
            foreach (var whereExpression in spec.WhereExpressions)
            {
                predicate = predicate.And(whereExpression.Filter);
            }
        }

        var tasks = await _unitOfWork.TaskRepository.GetAsync(predicate, cancellationToken);

        return _mapper.Map<IEnumerable<TaskResponse>>(tasks);
    }
}