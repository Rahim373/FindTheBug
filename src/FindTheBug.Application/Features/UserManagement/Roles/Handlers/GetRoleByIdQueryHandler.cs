using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;
using FindTheBug.Application.Features.UserManagement.Roles.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Roles.Handlers;

public class GetRoleByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetRoleByIdQuery, RoleResponseDto>
{
    public async Task<ErrorOr<RoleResponseDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Repository<Role>().GetQueryable()
            .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
        {
            return Error.NotFound("Role.NotFound", "Role not found.");
        }

        // Map to DTO
        var roleDto = new RoleResponseDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsActive = role.IsActive,
            IsSystemRole = role.IsSystemRole,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            ModulePermissions = role.RoleModulePermissions?.Select(rmp => new ModulePermissionDto
            {
                ModuleId = rmp.ModuleId,
                ModuleName = rmp.Module?.Name ?? string.Empty,
                CanView = rmp.CanView,
                CanCreate = rmp.CanCreate,
                CanEdit = rmp.CanEdit,
                CanDelete = rmp.CanDelete
            }).ToList()
        };

        return roleDto;
    }
}
