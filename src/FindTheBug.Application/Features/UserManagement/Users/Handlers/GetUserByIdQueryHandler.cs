using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;
using FindTheBug.Application.Features.UserManagement.Users.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Users.Handlers;

public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetUserByIdQuery, UserResponseDto>
{
    public async Task<ErrorOr<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Error.NotFound("User.NotFound", "User not found");

        return new UserResponseDto
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
            UpdatedAt = user.UpdatedAt,
            Roles = user.UserRoles?.Select(ur => new UserRoleDto
            {
                RoleId = ur.RoleId,
                RoleName = ur.Role?.Name ?? string.Empty
            }).ToList()
        };
    }
}
