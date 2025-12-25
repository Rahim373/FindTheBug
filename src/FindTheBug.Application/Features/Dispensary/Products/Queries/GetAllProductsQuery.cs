using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Products.Queries;

public record GetAllProductsQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<ProductListItemDto>>;
