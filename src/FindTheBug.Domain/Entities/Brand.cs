using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Brand : BaseAuditableEntity
{
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Drug> Drugs { get; set; } = new List<Drug>();
}
