using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class GetActiveRolesQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetActiveRolesQuery, IEnumerable<Role>>
{
    public async Task<ErrorOr<IEnumerable<Role>>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await unitOfWork.Repository<Role>()
            .GetQueryable()
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return roles;
    }
}
