using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Application.Features.Dispensary.Drugs.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class GetAllGenericNamesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllGenericNamesQuery, List<GenericNameDto>>
{
    public async Task<ErrorOr<List<GenericNameDto>>> Handle(GetAllGenericNamesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<GenericName>().GetQueryable()
            .Where(gn => gn.IsActive)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(gn => gn.Name.ToLower().Contains(searchTerm));
        }

        var genericNames = await query
            .OrderBy(gn => gn.Name)
            .Select(gn => new GenericNameDto
            {
                Id = gn.Id,
                Name = gn.Name,
                Description = gn.Description,
                IsActive = gn.IsActive
            })
            .ToListAsync(cancellationToken);

        return genericNames;
    }
}
