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
                    .ThenInclude(r => r.RoleModulePermissions)
                        .ThenInclude(rmp => rmp.Module)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user == null)
            return Error.NotFound("User.NotFound", "User not found");

        // Collect all permissions from all roles
        var permissions = new List<ModulePermissionDto>();
        if (user.UserRoles != null)
        {
            foreach (var userRole in user.UserRoles)
            {
                if (userRole.Role?.RoleModulePermissions != null)
                {
                    foreach (var roleModulePermission in userRole.Role.RoleModulePermissions)
                    {
                        var module = roleModulePermission.Module;
                        if (module != null && module.IsActive)
                        {
                            if (roleModulePermission.CanView)
                                permissions.Add(new ModulePermissionDto { Module = module.Name, Permission = "View" });
                            if (roleModulePermission.CanCreate)
                                permissions.Add(new ModulePermissionDto { Module = module.Name, Permission = "Create" });
                            if (roleModulePermission.CanEdit)
                                permissions.Add(new ModulePermissionDto { Module = module.Name, Permission = "Edit" });
                            if (roleModulePermission.CanDelete)
                                permissions.Add(new ModulePermissionDto { Module = module.Name, Permission = "Delete" });
                        }
                    }
                }
            }
        }

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
            }).ToList(),
            Permissions = permissions.Distinct().ToList()
        };
    }
}