using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Users.Queries;
using FindTheBug.Domain.Contracts;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Handlers;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetAllUsersQuery, PagedResult<User>>
{
    public async Task<ErrorOr<PagedResult<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<User>().GetQueryable()
            .Where(u => !u.UserRoles.Any(ur => ur.Role.Name == RoleConstants.SuperUser));

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower) ||
                (u.Phone != null && u.Phone.Contains(request.Search)) ||
                (u.NIDNumber != null && u.NIDNumber.Contains(request.Search)));
        }

        // Order by creation date (newest first)
        query = query.OrderByDescending(u => u.CreatedAt);

        // Create paginated result
        var pagedResult = PagedResult<User>.Create(query, request.PageNumber, request.PageSize);

        return pagedResult;
    }
}
