using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Dispensary.Products.DTOs;

namespace FindTheBug.Application.Features.Dispensary.Products.Commands;

public record CreateProductCommand(
    string Name,
    decimal Price,
    int Quantity,
    string? Description,
    string? PhotoPath
) : ICommand<ProductResponseDto>;
