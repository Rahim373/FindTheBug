using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.DiagnosticTests.Commands;

public record CreateDiagnosticTestCommand(
    string TestCode,
    string TestName,
    string Description,
    string Category,
    decimal Price,
    int DurationInHours,
    bool RequiresFasting
) : IRequest<Result<DiagnosticTest>>;

public class CreateDiagnosticTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateDiagnosticTestCommand, Result<DiagnosticTest>>
{
    public async Task<Result<DiagnosticTest>> Handle(CreateDiagnosticTestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var test = new DiagnosticTest
            {
                Id = Guid.NewGuid(),
                TenantId = string.Empty, // Will be set by DbContext
                TestCode = request.TestCode,
                TestName = request.TestName,
                Description = request.Description,
                Category = request.Category,
                Price = request.Price,
                DurationInHours = request.DurationInHours,
                RequiresFasting = request.RequiresFasting,
                IsActive = true
            };

            var created = await unitOfWork.Repository<DiagnosticTest>().AddAsync(test, cancellationToken);
            return Result<DiagnosticTest>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<DiagnosticTest>.Failure($"Error creating diagnostic test: {ex.Message}");
        }
    }
}
