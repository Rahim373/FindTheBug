using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Features.Modules.Queries;
using FindTheBug.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Modules.Handlers;

public class GetAllModulesQueryHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<GetAllModulesQuery, List<ModuleDto>>
{
    public async Task<List<ModuleDto>> Handle(GetAllModulesQuery request, CancellationToken cancellationToken)
    {
        var modules = await unitOfWork.Repository<Module>()
            .GetQueryable()
            .Where(m => m.IsActive)
            .OrderBy(m => m.Name)
            .Select(m => new ModuleDto(
                m.Id,
                m.Name,
                m.DisplayName,
                m.Description,
                m.IsActive
            ))
            .ToListAsync(cancellationToken);

        return modules;
    }
}
