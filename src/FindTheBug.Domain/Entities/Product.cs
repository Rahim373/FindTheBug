using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; } = true;
}
