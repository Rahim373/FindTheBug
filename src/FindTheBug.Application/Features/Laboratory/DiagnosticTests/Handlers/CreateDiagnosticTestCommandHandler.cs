using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class CreateDiagnosticTestCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateDiagnosticTestCommand, DiagnosticTestResponseDto>
{
    public async Task<ErrorOr<Result<DiagnosticTestResponseDto>>> Handle(CreateDiagnosticTestCommand request, CancellationToken cancellationToken)
    {
        var test = new DiagnosticTest
        {
            TestName = request.TestName,
            Price = request.Price,
            Description = request.Description
        };

        var created = await unitOfWork.Repository<DiagnosticTest>().AddAsync(test, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DiagnosticTestResponseDto>.Success(new DiagnosticTestResponseDto
        {
            Id = created.Id,
            TestName = created.TestName,
            Price = created.Price,
            Description = created.Description,
            CreatedAt = created.CreatedAt
        });
    }
}
