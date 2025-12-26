using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Module : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<RoleModulePermission> RoleModulePermissions { get; set; } = new List<RoleModulePermission>();
}
