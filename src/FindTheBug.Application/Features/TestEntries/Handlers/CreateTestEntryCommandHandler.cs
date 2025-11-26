using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.TestEntries.Commands;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestEntries.Handlers;

public class CreateTestEntryCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateTestEntryCommand, TestEntry>
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
