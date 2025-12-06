using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Roles.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Handlers;

public class GetAllRolesQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetAllRolesQuery, PagedResult<Role>>
{
    public async Task<ErrorOr<PagedResult<Role>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Role>().GetQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(searchLower) ||
                (r.Description != null && r.Description.ToLower().Contains(searchLower)));
        }

        // Order by name
        query = query.OrderBy(r => r.Name);

        // Create paginated result
        var pagedResult = PagedResult<Role>.Create(query, request.PageNumber, request.PageSize);

        return pagedResult;
    }
}
