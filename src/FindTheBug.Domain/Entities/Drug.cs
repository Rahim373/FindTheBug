using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public enum DrugType
{
    Syrup,
    Tablet,
    Capsule,
    Gel,
    Ointment,
    Suspension,
    Injection,
    Cream,
    Drops,
    Powder
}

public class Drug : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string Strength { get; set; } = string.Empty;
    public Guid GenericNameId { get; set; }
    public Guid BrandId { get; set; }
    public DrugType Type { get; set; }
    public decimal UnitPrice { get; set; }
    public string? PhotoPath { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public GenericName GenericName { get; set; } = null!;
    public Brand Brand { get; set; } = null!;
}
