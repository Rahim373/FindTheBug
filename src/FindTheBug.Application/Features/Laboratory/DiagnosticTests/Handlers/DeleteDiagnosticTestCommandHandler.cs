using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Laboratory.DiagnosticTests.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Laboratory.DiagnosticTests.Handlers;

public class DeleteDiagnosticTestCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteDiagnosticTestCommand, bool>
{
    public async Task<ErrorOr<Result<bool>>> Handle(DeleteDiagnosticTestCommand request, CancellationToken cancellationToken)
    {
        var test = await unitOfWork.Repository<DiagnosticTest>()
            .GetQueryable()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (test == null)
            return Error.NotFound("DiagnosticTest.NotFound", "Diagnostic test not found");

        await unitOfWork.Repository<DiagnosticTest>().DeleteAsync(test.Id, cancellationToken);

        return Result<bool>.Success(true);
    }
}