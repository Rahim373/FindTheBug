using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.TestParameters.Commands;

public class DeleteTestParameterCommandHandler(IUnitOfWork unitOfWork) 
    : ICommandHandler<DeleteTestParameterCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteTestParameterCommand request, CancellationToken cancellationToken)
    {
        var existing = await unitOfWork.Repository<TestParameter>().GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            return Error.NotFound("TestParameter.NotFound", $"Test parameter with ID {request.Id} not found");

        await unitOfWork.Repository<TestParameter>().DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
