using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Users.Commands;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Users.Handlers;

public class CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, UserResponseDto>
{
    public async Task<ErrorOr<UserResponseDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email exists
        if (!string.IsNullOrEmpty(request.Email))
        {
            var existing = await unitOfWork.Repository<User>().GetQueryable()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (existing != null)
                return Error.Conflict("User.EmailExists", "Email already exists");
        }

        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            NIDNumber = request.NIDNumber,
            IsActive = request.IsActive,
            AllowUserLogin = request.AllowUserLogin
        };

        var created = await unitOfWork.Repository<User>().AddAsync(user, cancellationToken);

        // Add roles
        foreach (var roleId in request.RoleIds)
        {
            await unitOfWork.Repository<UserRole>().AddAsync(new UserRole
            {
                UserId = created.Id,
                RoleId = roleId
            }, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserResponseDto
        {
            Id = created.Id,
            Email = created.Email,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Phone = created.Phone,
            NIDNumber = created.NIDNumber,
            IsActive = created.IsActive,
            AllowUserLogin = created.AllowUserLogin,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
