using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Dispensary.Drugs.DTOs;

public record DrugResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Strength { get; init; } = string.Empty;
    public GenericNameDto GenericName { get; init; } = null!;
    public BrandDto Brand { get; init; } = null!;
    public DrugType Type { get; init; }
    public decimal UnitPrice { get; init; }
    public string? PhotoPath { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record DrugListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Strength { get; init; } = string.Empty;
    public string GenericName { get; init; } = string.Empty;
    public string BrandName { get; init; } = string.Empty;
    public DrugType Type { get; init; }
    public decimal UnitPrice { get; init; }
    public bool IsActive { get; init; }
}

public record BrandDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

public record GenericNameDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}

public record CreateDrugCommandDto
{
    public required string Name { get; init; }
    public required string Strength { get; init; }
    public required Guid GenericNameId { get; init; }
    public required Guid BrandId { get; init; }
    public required DrugType Type { get; init; }
    public required decimal UnitPrice { get; init; }
    public string? PhotoPath { get; init; }
}

public record UpdateDrugCommandDto
{
    public required string Name { get; init; }
    public required string Strength { get; init; }
    public required Guid GenericNameId { get; init; }
    public required Guid BrandId { get; init; }
    public required DrugType Type { get; init; }
    public required decimal UnitPrice { get; init; }
    public string? PhotoPath { get; init; }
    public bool IsActive { get; init; }
}
