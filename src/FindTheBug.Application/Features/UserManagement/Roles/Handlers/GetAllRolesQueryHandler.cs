using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;
using FindTheBug.Application.Features.UserManagement.Roles.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Roles.Handlers;

public class GetAllRolesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllRolesQuery, PagedResult<RoleListItemDto>>
{
    public async Task<ErrorOr<Result<PagedResult<RoleListItemDto>>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Role> query = unitOfWork.Repository<Role>().GetQueryable()
            .Include(r => r.RoleModulePermissions);

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

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var roles = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var roleDtos = roles.Select(r => new RoleListItemDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsActive = r.IsActive,
            IsSystemRole = r.IsSystemRole,
            ModuleCount = r.RoleModulePermissions?.Count ?? 0
        }).ToList();

        return Result<PagedResult<RoleListItemDto>>.Success(new PagedResult<RoleListItemDto>
        {
            Items = roleDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
