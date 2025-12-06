using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<UpdateRoleCommand, Role>
{
    public async Task<ErrorOr<Role>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Repository<Role>().GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
        {
            return Error.NotFound("Role.NotFound", "Role not found.");
        }

        // Prevent modification of system roles
        if (role.IsSystemRole)
        {
            return Error.Forbidden("Role.SystemRole", "System roles cannot be modified.");
        }

        // Check if new name conflicts with existing role
        if (role.Name != request.Name)
        {
            var existingRole = await unitOfWork.Repository<Role>()
                .GetQueryable()
                .FirstOrDefaultAsync(r => r.Name == request.Name && r.Id != request.Id, cancellationToken);

            if (existingRole != null)
            {
                return Error.Conflict("Role.NameExists", "A role with this name already exists.");
            }
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsActive = request.IsActive;

        await unitOfWork.Repository<Role>().UpdateAsync(role, cancellationToken);
        return role;
    }
}
