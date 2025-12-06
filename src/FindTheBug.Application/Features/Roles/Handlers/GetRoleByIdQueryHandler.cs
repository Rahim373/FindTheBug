using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class GetRoleByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetRoleByIdQuery, Role>
{
    public async Task<ErrorOr<Role>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Repository<Role>().GetByIdAsync(request.Id, cancellationToken);

        if (role == null)
        {
            return Error.NotFound("Role.NotFound", "Role not found.");
        }

        return role;
    }
}
