using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Products.Commands;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Products.Handlers;

public class UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand, ProductResponseDto>
{
    public async Task<ErrorOr<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>().GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Error.NotFound("Product.NotFound", "Product not found");

        product.Name = request.Name;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.Description = request.Description;
        product.PhotoPath = request.PhotoPath;
        product.IsActive = request.IsActive;

        await unitOfWork.Repository<Product>().UpdateAsync(product, cancellationToken);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Description = product.Description,
            PhotoPath = product.PhotoPath,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
