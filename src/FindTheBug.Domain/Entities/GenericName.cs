using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class GenericName : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}
