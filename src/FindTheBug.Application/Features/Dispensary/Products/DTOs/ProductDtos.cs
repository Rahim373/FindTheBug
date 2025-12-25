namespace FindTheBug.Application.Features.Dispensary.Products.DTOs;

public record ProductResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public string? Description { get; init; }
    public string? PhotoPath { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record ProductListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public record CreateProductCommandDto
{
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
    public string? Description { get; init; }
    public string? PhotoPath { get; init; }
}

public record UpdateProductCommandDto
{
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required int Quantity { get; init; }
    public string? Description { get; init; }
    public string? PhotoPath { get; init; }
    public bool IsActive { get; init; }
}
