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
        // Validate email uniqueness if email is provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingUser = await unitOfWork.Repository<User>()
                .GetQueryable()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (existingUser != null)
            {
                return Error.Conflict("User.EmailExists", "A user with this email already exists.");
            }
        }

        // Hash the password
        var passwordHash = passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            NIDNumber = request.NIDNumber,
            Roles = request.Roles ?? "User",
            IsActive = request.IsActive
        };

        var created = await unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        return created;
    }
}
