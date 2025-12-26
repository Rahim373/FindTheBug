using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Users.Commands;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Users.Handlers;

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    : ICommandHandler<UpdateUserCommand, UserResponseDto>
{
    public async Task<ErrorOr<Result<UserResponseDto>>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Error.NotFound("User.NotFound", "User not found");

        // Check email conflict
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var existing = await unitOfWork.Repository<User>().GetQueryable()
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != request.Id, cancellationToken);
            if (existing != null)
                return Error.Conflict("User.EmailExists", "Email already exists");
        }

        user.Email = request.Email;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        user.NIDNumber = request.NIDNumber;
        user.IsActive = request.IsActive;
        user.AllowUserLogin = request.AllowUserLogin;

        if (!string.IsNullOrEmpty(request.Password))
            user.PasswordHash = passwordHasher.HashPassword(request.Password);

        // Update roles
        var existingRoles = user.UserRoles.ToList();
        foreach (var role in existingRoles)
            await unitOfWork.Repository<UserRole>().DeleteAsync(role.Id);

        foreach (var roleId in request.RoleIds)
        {
            await unitOfWork.Repository<UserRole>().AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            }, cancellationToken);
        }

        await unitOfWork.Repository<User>().UpdateAsync(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserResponseDto>.Success(new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Phone = user.Phone,
            NIDNumber = user.NIDNumber,
            IsActive = user.IsActive,
            AllowUserLogin = user.AllowUserLogin,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }
}
