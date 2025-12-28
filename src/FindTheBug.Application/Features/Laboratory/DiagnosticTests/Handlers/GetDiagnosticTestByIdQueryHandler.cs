using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class GetDiagnosticTestByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetDiagnosticTestByIdQuery, DiagnosticTestResponseDto>
{
    public async Task<ErrorOr<Result<DiagnosticTestResponseDto>>> Handle(GetDiagnosticTestByIdQuery request, CancellationToken cancellationToken)
    {
        var test = await unitOfWork.Repository<DiagnosticTest>()
            .GetQueryable()
            .Where(d => d.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (test == null)
        {
            return Error.NotFound($"Diagnostic test with ID {request.Id} not found");
        }

        return Result<DiagnosticTestResponseDto>.Success(test);
    }
}