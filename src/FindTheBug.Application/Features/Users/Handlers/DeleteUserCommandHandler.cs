using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Handlers;

public class DeleteUserCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Repository<User>().GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        await unitOfWork.Repository<User>().DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
