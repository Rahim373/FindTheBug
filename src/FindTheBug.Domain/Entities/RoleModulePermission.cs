using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class RoleModulePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid ModuleId { get; set; }
    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
