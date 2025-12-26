using FindTheBug.Domain.Common;

namespace FindTheBug.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleModulePermission> RoleModulePermissions { get; set; } = new List<RoleModulePermission>();
}
