using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class CreateRoleCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<CreateRoleCommand, Role>
{
    public async Task<ErrorOr<Role>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        // Check if role name already exists
        var existingRole = await unitOfWork.Repository<Role>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Name == request.Name, cancellationToken);

        if (existingRole != null)
        {
            return Error.Conflict("Role.NameExists", "A role with this name already exists.");
        }

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive,
            IsSystemRole = false
        };

        var created = await unitOfWork.Repository<Role>().AddAsync(role, cancellationToken);
        return created;
    }
}
