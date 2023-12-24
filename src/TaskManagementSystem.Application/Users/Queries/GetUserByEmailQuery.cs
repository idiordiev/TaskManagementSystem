using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Models;

namespace TaskManagementSystem.Application.Users.Queries;

public class GetUserByEmailQuery : IRequest<UserResponse?>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponse?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(request.Email, cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }
}
