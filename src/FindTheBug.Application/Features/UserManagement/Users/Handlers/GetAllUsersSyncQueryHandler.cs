using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Users.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Users.Handlers;

public class GetAllUsersSyncQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllUsersSyncQuery, PagedResult<User>>
{
    public async Task<ErrorOr<Result<PagedResult<User>>>> Handle(GetAllUsersSyncQuery request, CancellationToken cancellationToken)
    {
        IQueryable<User> query = unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role.RoleModulePermissions)
            .ThenInclude(rmp => rmp.Module)
            .AsNoTracking();

        query = query.OrderBy(u => u.CreatedBy);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        users.ForEach(user =>
        {
            foreach (var role in user.UserRoles)
            {
                role.User = null;
                role.Role.UserRoles = null;

                foreach (var rolePermission in role.Role.RoleModulePermissions)
                {
                    rolePermission.Role = null;
                    rolePermission.Module.RoleModulePermissions = null;
                }
            }
        });

        return Result<PagedResult<User>>.Success(new PagedResult<User>
        {
            Items = users,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
