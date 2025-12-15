using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class CreateDiagnosticTestCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDiagnosticTestCommand, DiagnosticTestResponseDto>
{
    public async Task<ErrorOr<DiagnosticTestResponseDto>> Handle(CreateDiagnosticTestCommand request, CancellationToken cancellationToken)
    {
        var test = new DiagnosticTest
        {
            TestName = request.TestName,
            TestCode = request.TestCode,
            Price = request.Price,
            Description = request.Description
        };

        var created = await unitOfWork.Repository<DiagnosticTest>().AddAsync(test, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DiagnosticTestResponseDto
        {
            Id = created.Id,
            TestName = created.TestName,
            TestCode = created.TestCode,
            Price = created.Price,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        };
    }
}
