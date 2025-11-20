using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.TestEntries.Commands;

public record CreateTestEntryCommand(
    Guid PatientId,
    Guid DiagnosticTestId,
    DateTime? SampleCollectionDate,
    string Priority,
    string? ReferredBy,
    string? Notes
) : IRequest<ErrorOr<TestEntry>>;

public class CreateTestEntryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTestEntryCommand, ErrorOr<TestEntry>>
{
    public async Task<ErrorOr<TestEntry>> Handle(CreateTestEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = new TestEntry
        {
            Id = Guid.NewGuid(),
            TenantId = string.Empty, // Will be set by DbContext
            PatientId = request.PatientId,
            DiagnosticTestId = request.DiagnosticTestId,
            EntryNumber = $"TE-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            EntryDate = DateTime.UtcNow,
            SampleCollectionDate = request.SampleCollectionDate,
            Status = "Registered",
            Priority = request.Priority,
            ReferredBy = request.ReferredBy,
            Notes = request.Notes
        };

        var created = await unitOfWork.Repository<TestEntry>().AddAsync(entry, cancellationToken);
        return created;
    }
}
