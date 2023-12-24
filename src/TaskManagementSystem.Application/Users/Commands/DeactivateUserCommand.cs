using MediatR;
using TaskManagementSystem.Application.Exceptions;
using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Application.Users.Commands;

public class DeactivateUserCommand : IRequest
{
    public int UserId { get; set; }
}

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public DeactivateUserCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", request.UserId);
        }

        await _identityService.DeleteAccountsForUserAsync(user.Id, cancellationToken);

        user.State = UserState.Deleted;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}