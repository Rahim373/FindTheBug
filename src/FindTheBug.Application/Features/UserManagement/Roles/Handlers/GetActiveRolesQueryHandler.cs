using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;
using FindTheBug.Application.Features.UserManagement.Roles.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Roles.Handlers;

public class GetActiveRolesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetActiveRolesQuery, List<RoleListItemDto>>
{
    public async Task<ErrorOr<List<RoleListItemDto>>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await unitOfWork.Repository<Role>().GetQueryable()
            .Include(r => r.RoleModulePermissions)
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
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

        return roleDtos;
    }
}
