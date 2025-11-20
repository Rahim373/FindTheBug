using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.DiagnosticTests.Commands;

public record CreateDiagnosticTestCommand(
    string TestCode,
    string TestName,
    string? Description,
    string Category,
    decimal Price,
    int? Duration,
    bool RequiresFasting
) : IRequest<ErrorOr<DiagnosticTest>>;

public class CreateDiagnosticTestCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateDiagnosticTestCommand, ErrorOr<DiagnosticTest>>
{
    public async Task<ErrorOr<DiagnosticTest>> Handle(CreateDiagnosticTestCommand request, CancellationToken cancellationToken)
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
            Duration = request.Duration,
            RequiresFasting = request.RequiresFasting,
            IsActive = true
        };

        var created = await unitOfWork.Repository<DiagnosticTest>().AddAsync(test, cancellationToken);
        return created;
    }
}
