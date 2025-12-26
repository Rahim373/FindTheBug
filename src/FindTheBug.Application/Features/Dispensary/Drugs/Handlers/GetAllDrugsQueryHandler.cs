using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;
using FindTheBug.Application.Features.Dispensary.Drugs.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Handlers;

public class GetAllDrugsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllDrugsQuery, PagedResult<DrugListItemDto>>
{
    public async Task<ErrorOr<Result<PagedResult<DrugListItemDto>>>> Handle(GetAllDrugsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Drug>().GetQueryable()
            .Include(d => d.GenericName)
            .Include(d => d.Brand)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(searchTerm) ||
                d.GenericName.Name.ToLower().Contains(searchTerm) ||
                d.Brand.Name.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var drugs = await query
            .OrderBy(d => d.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DrugListItemDto
            {
                Id = d.Id,
                Name = d.Name,
                Strength = d.Strength,
                GenericName = d.GenericName.Name,
                BrandName = d.Brand.Name,
                Type = d.Type,
                UnitPrice = d.UnitPrice,
                IsActive = d.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<DrugListItemDto>>.Success(new PagedResult<DrugListItemDto>
        {
            Items = drugs,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
