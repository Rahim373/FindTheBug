using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductResponseDto>;
