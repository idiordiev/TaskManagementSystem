using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Models;

namespace TaskManagementSystem.Application.Users.Commands;

public class UpdateUserCommand : IRequest<UserResponse>
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", request.Id);
        }

        _mapper.Map(request, user);
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserResponse>(user);
    }
}