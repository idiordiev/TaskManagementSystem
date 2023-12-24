using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Models;

namespace TaskManagementSystem.Application.Users.Queries;

public class GetUserByIdQuery : IRequest<UserResponse?>
{
    public int UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }
}