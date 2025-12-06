using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class DeleteRoleCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<DeleteRoleCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Repository<Role>().GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
        {
            return Error.NotFound("Role.NotFound", "Role not found.");
        }

        // Prevent deletion of system roles
        if (role.IsSystemRole)
        {
            return Error.Forbidden("Role.SystemRole", "System roles cannot be deleted.");
        }

        await unitOfWork.Repository<Role>().DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
