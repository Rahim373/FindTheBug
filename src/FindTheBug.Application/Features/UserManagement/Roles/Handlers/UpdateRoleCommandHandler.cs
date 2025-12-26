using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Roles.Commands;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Roles.Handlers;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateRoleCommand, RoleResponseDto>
{
    public async Task<ErrorOr<Result<RoleResponseDto>>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Repository<Role>()
            .GetQueryable()
            .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (role == null)
        {
            return Error.NotFound("Role.NotFound", "Role not found.");
        }

        if (role.IsSystemRole)
        {
            return Error.Validation("Role.SystemRole", "System roles cannot be modified.");
        }

        // Check if new name conflicts with existing role
        var existingRole = await unitOfWork.Repository<Role>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == request.Name && r.Id != request.Id, cancellationToken);

        if (existingRole != null)
        {
            return Error.Conflict("Role.NameExists", "A role with this name already exists.");
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Repository<Role>().UpdateAsync(role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Return DTO
        return Result<RoleResponseDto>.Success(new RoleResponseDto
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
        });
    }
}
