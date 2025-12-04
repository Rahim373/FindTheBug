using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Handlers;

public class UpdateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) 
    : ICommandHandler<UpdateUserCommand, User>
{
    public async Task<ErrorOr<User>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Repository<User>().GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        // Update fields
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.NIDNumber = request.NIDNumber;
        user.Roles = request.Roles ?? user.Roles;
        user.IsActive = request.IsActive;

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = passwordHasher.HashPassword(request.Password);
        }

        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);
        return user;
    }
}
