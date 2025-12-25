using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Drugs.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Drugs.Queries;

public record GetAllDrugsQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<DrugListItemDto>>;
