using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;
using FindTheBug.Application.Features.UserManagement.Users.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Users.Handlers;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllUsersQuery, PagedResult<UserListItemDto>>
{
    public async Task<ErrorOr<Result<PagedResult<UserListItemDto>>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<User> query = unitOfWork.Repository<User>().GetQueryable()
            .Include(u => u.UserRoles);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower) ||
                (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                u.Phone.Contains(searchLower));
        }

        query = query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userDtos = users.Select(u => new UserListItemDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            LastLoginAt = u.LastLoginAt,
            Phone = u.Phone,
            IsActive = u.IsActive,
            RoleCount = u.UserRoles?.Count ?? 0
        }).ToList();

        return Result<PagedResult<UserListItemDto>>.Success(new PagedResult<UserListItemDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
