using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Reception.DTOs;

namespace FindTheBug.Application.Features.Reception.Queries;

public record GetAllReceiptsQuery(
    string? Search,
    int PageNumber,
    int PageSize
) : IQuery<Common.Models.PagedResult<ReceiptListItemDto>>;