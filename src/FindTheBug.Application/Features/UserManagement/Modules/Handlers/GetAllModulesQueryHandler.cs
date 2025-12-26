using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.UserManagement.Modules.DTOs;
using FindTheBug.Application.Features.UserManagement.Modules.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.UserManagement.Modules.Handlers;

public class GetAllModulesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllModulesQuery, List<ModuleDto>>
{
    public async Task<ErrorOr<Result<List<ModuleDto>>>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await unitOfWork.Repository<Module>().GetQueryable()
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        var moduleDtos = modules.Select(m => new ModuleDto
        {
            Id = m.Id,
            Name = m.Name,
            DisplayName = m.DisplayName,
            Description = m.Description,
            IsActive = m.IsActive
        }).ToList();

        return Result<List<ModuleDto>>.Success(moduleDtos);
    }
}
