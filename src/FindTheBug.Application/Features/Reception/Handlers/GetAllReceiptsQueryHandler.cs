using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Reception.DTOs;
using FindTheBug.Application.Features.Reception.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Reception.Handlers;

public class GetAllReceiptsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllReceiptsQuery, PagedResult<ReceiptListItemDto>>
{
    public async Task<ErrorOr<Result<PagedResult<ReceiptListItemDto>>>> Handle(GetAllReceiptsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<LabReceipt>().GetQueryable()
            .Include(r => r.ReferredBy)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(r =>
                r.InvoiceNumber.ToLower().Contains(searchTerm) ||
                r.FullName.ToLower().Contains(searchTerm) ||
                r.PhoneNumber.Contains(searchTerm) ||
                (r.Address != null && r.Address.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var receipts = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReceiptListItemDto
            {
                Id = r.Id,
                InvoiceNumber = r.InvoiceNumber,
                FullName = r.FullName,
                PhoneNumber = r.PhoneNumber,
                Total = r.Total,
                Due = r.Due,
                Status = (int)r.LabReceiptStatus,
                StatusDisplay = r.LabReceiptStatus.ToString(),
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ReceiptListItemDto>>.Success(new PagedResult<ReceiptListItemDto>
        {
            Items = receipts,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}