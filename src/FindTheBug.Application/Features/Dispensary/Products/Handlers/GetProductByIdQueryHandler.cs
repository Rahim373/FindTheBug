using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;
using FindTheBug.Application.Features.Dispensary.Products.Queries;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FindTheBug.Application.Features.Dispensary.Products.Handlers;

public class GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetProductByIdQuery, ProductResponseDto>
{
    public async Task<ErrorOr<Result<ProductResponseDto>>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>().GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            return Error.NotFound("Product.NotFound", "Product not found");

        return Result<ProductResponseDto>.Success(new ProductResponseDto
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
        });
    }
}
