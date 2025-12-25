using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Products.Commands;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Products.Handlers;

public class DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductCommand, bool>
{
    public async Task<ErrorOr<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>().GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Error.NotFound("Product.NotFound", "Product not found");

        await unitOfWork.Repository<Product>().DeleteAsync(request.Id, cancellationToken);

        return true;
    }
}
