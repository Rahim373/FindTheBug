using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Products.Commands;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Dispensary.Products.Handlers;

public class CreateProductCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProductCommand, ProductResponseDto>
{
    public async Task<ErrorOr<Result<ProductResponseDto>>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            Quantity = request.Quantity,
            Description = request.Description,
            PhotoPath = request.PhotoPath,
            IsActive = true
        };

        var created = await unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);

        return Result<ProductResponseDto>.Success(new ProductResponseDto
        {
            Id = created.Id,
            Name = created.Name,
            Price = created.Price,
            Quantity = created.Quantity,
            Description = created.Description,
            PhotoPath = created.PhotoPath,
            IsActive = created.IsActive,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        });
    }
}
