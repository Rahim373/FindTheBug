using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class GetAllDiagnosticTestsQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetAllDiagnosticTestsQuery, PagedResult<DiagnosticTestResponseDto>>
{
    public async Task<ErrorOr<Result<PagedResult<DiagnosticTestResponseDto>>>> Handle(GetAllDiagnosticTestsQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.Repository<DiagnosticTest>().GetQueryable().AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(d =>
                d.TestName.ToLower().Contains(searchTerm) ||
                (d.Category != null && d.Category.ToLower().Contains(searchTerm)));
        }

        // Apply category filter if provided
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(d => d.Category.ToLower() == request.Category.ToLower());
        }

        // Apply active filter if provided
        if (request.IsActive.HasValue)
        {
            query = query.Where(d => d.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var tests = await query
            .OrderBy(d => d.Category)
            .ThenBy(d => d.TestName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DiagnosticTestResponseDto
            {
                Id = d.Id,
                TestName = d.TestName,
                Category = d.Category,
                Price = d.Price,
                Description = d.Description,
                Duration = d.Duration,
                RequiresFasting = d.RequiresFasting,
                IsActive = d.IsActive,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<DiagnosticTestResponseDto>>.Success(new PagedResult<DiagnosticTestResponseDto>
        {
            Items = tests,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}