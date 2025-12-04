using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        // Validate phone uniqueness if phone is being changed
        if (user.Phone != request.Phone)
        {
            var existingUserByPhone = await unitOfWork.Repository<User>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Phone == request.Phone && u.Id != request.Id, cancellationToken);

            if (existingUserByPhone != null)
            {
                return Error.Conflict("User.PhoneExists", "A user with this phone number already exists.");
            }
        }

        // Update fields
        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.NIDNumber = request.NIDNumber;
        user.Roles = request.Roles ?? user.Roles;
        user.IsActive = request.IsActive;
        user.AllowUserLogin = request.AllowUserLogin;

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = passwordHasher.HashPassword(request.Password);
        }

        await unitOfWork.Repository<User>().UpdateAsync(user, cancellationToken);
        return user;
    }
}
