using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;
using FindTheBug.Application.Features.Dispensary.Products.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Products.Handlers;

public class GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllProductsQuery, PagedResult<ProductListItemDto>>
{
    public async Task<ErrorOr<Result<PagedResult<ProductListItemDto>>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<Product>().GetQueryable()
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .OrderBy(p => p.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity,
                Description = p.Description,
                IsActive = p.IsActive
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ProductListItemDto>>.Success(new PagedResult<ProductListItemDto>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
