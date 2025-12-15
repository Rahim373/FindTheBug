using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.TestEntries.Commands;
using FindTheBug.Application.Features.Laboratory.TestEntries.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.TestEntries.Handlers;

public class CreateTestEntryCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTestEntryCommand, TestEntryResponseDto>
{
    public async Task<ErrorOr<TestEntryResponseDto>> Handle(CreateTestEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = new TestEntry
        {
            PatientId = request.PatientId,
            DiagnosticTestId = request.DiagnosticTestId,
            EntryDate = request.EntryDate,
            Status = "Pending"
        };

        var created = await unitOfWork.Repository<TestEntry>().AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Load related entities
        var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(created.PatientId, cancellationToken);
        var test = await unitOfWork.Repository<DiagnosticTest>().GetByIdAsync(created.DiagnosticTestId, cancellationToken);

        return new TestEntryResponseDto
        {
            Id = created.Id,
            PatientId = created.PatientId,
            PatientName = patient?.FirstName ?? string.Empty,
            DiagnosticTestId = created.DiagnosticTestId,
            TestName = test?.TestName ?? string.Empty,
            EntryDate = created.EntryDate,
            Status = created.Status,
            CreatedAt = created.CreatedAt
        };
    }
}
