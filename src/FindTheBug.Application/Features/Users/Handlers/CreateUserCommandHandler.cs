using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Users.Handlers;

public class CreateUserCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) 
    : ICommandHandler<CreateUserCommand, User>
{
    public async Task<ErrorOr<User>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validate phone uniqueness (required field)
        var existingUserByPhone = await unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Phone == request.Phone, cancellationToken);

        if (existingUserByPhone != null)
        {
            return Error.Conflict("User.PhoneExists", "A user with this phone number already exists.");
        }

        // Validate email uniqueness if email is provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingUserByEmail = await unitOfWork.Repository<User>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUserByEmail != null)
            {
                return Error.Conflict("User.EmailExists", "A user with this email already exists.");
            }
        }

        // Hash the password
        var passwordHash = passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone!,
            NIDNumber = request.NIDNumber,
            IsActive = request.IsActive,
            AllowUserLogin = request.AllowUserLogin
        };

        var created = await unitOfWork.Repository<User>().AddAsync(user, cancellationToken);

        // Create UserRole entries
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = created.Id,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow
                };
                await unitOfWork.Repository<UserRole>().AddAsync(userRole, cancellationToken);
            }
        }

        return created;
    }
}
