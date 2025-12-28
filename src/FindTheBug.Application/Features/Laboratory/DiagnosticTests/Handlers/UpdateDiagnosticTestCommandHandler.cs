using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class UpdateDiagnosticTestCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateDiagnosticTestCommand, DiagnosticTestResponseDto>
{
    public async Task<ErrorOr<Result<DiagnosticTestResponseDto>>> Handle(UpdateDiagnosticTestCommand request, CancellationToken cancellationToken)
    {
        var test = await unitOfWork.Repository<DiagnosticTest>()
            .GetQueryable()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (test == null)
            return Error.NotFound("DiagnosticTest.NotFound", "Diagnostic test not found");

        // Check if another test has the same test code
        if (!string.IsNullOrWhiteSpace(request.TestCode))
        {
            var existingWithCode = await unitOfWork.Repository<DiagnosticTest>()
                .GetQueryable()
                .FirstOrDefaultAsync(d => d.Id != request.Id, cancellationToken);

            if (existingWithCode != null)
                return Error.Conflict("DiagnosticTest.CodeExists", "Another diagnostic test with this code already exists");
        }

        // Update test properties
        test.TestName = request.TestName;
        test.Category = request.Category;
        test.Price = request.Price;
        test.Description = request.Description;
        test.Duration = request.Duration;
        test.RequiresFasting = request.RequiresFasting;
        test.IsActive = request.IsActive;

        await unitOfWork.Repository<DiagnosticTest>().UpdateAsync(test, cancellationToken);

        return Result<DiagnosticTestResponseDto>.Success(new DiagnosticTestResponseDto
        {
            Id = test.Id,
            TestName = test.TestName,
            Category = test.Category,
            Price = test.Price,
            Description = test.Description,
            Duration = test.Duration,
            RequiresFasting = test.RequiresFasting,
            IsActive = test.IsActive,
            CreatedAt = test.CreatedAt
        });
    }
}