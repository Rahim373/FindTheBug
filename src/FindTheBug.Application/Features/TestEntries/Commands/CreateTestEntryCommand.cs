using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;
using MediatR;

namespace FindTheBug.Application.Features.TestEntries.Commands;

public record CreateTestEntryCommand(
    Guid PatientId,
    Guid DiagnosticTestId,
    string Priority,
    string? ReferredBy,
    string? Notes
) : IRequest<Result<TestEntry>>;

public class CreateTestEntryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateTestEntryCommand, Result<TestEntry>>
{
    public async Task<Result<TestEntry>> Handle(CreateTestEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entry = new TestEntry
            {
                Id = Guid.NewGuid(),
                TenantId = string.Empty, // Will be set by DbContext
                PatientId = request.PatientId,
                DiagnosticTestId = request.DiagnosticTestId,
                EntryNumber = $"TE-{DateTime.UtcNow:yyyy}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                EntryDate = DateTime.UtcNow,
                Status = "Registered",
                Priority = request.Priority,
                ReferredBy = request.ReferredBy,
                Notes = request.Notes
            };

            var created = await unitOfWork.Repository<TestEntry>().AddAsync(entry, cancellationToken);
            return Result<TestEntry>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<TestEntry>.Failure($"Error creating test entry: {ex.Message}");
        }
    }
}
