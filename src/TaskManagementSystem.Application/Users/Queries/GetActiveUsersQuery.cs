using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Users.Queries;

public class GetActiveUsersQuery : IRequest<IEnumerable<UserResponse>>
{
}

public class GetActiveUsersQueryHandler : IRequestHandler<GetActiveUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActiveUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponse>> Handle(GetActiveUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.UserRepository.GetAsync(x => x.State != UserState.Deleted, cancellationToken);

        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }
}