using System.ComponentModel.DataAnnotations;
using AutoMapper;
using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Users.Models;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Users.Commands;

public class CreateUserCommand : IRequest<UserResponse>
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(10)]
    [MaxLength(64)]
    public string Password { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.UserRepository.CheckIfActiveUserWithSameEmailExistsAsync(request.Email,
                cancellationToken))
        {
            throw new UserExistsException($"User with email {request.Email} already exists");
        }
        
        var user = new UserEntity
        {
            Name = request.Name,
            Email = request.Email,
        };

        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _identityService.CreateAccountAsync(request.Email, request.Password, user.Id);
        }
        catch (Exception)
        {
            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw;
        }

        return _mapper.Map<UserResponse>(user);
    }
}