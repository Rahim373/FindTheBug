using System.ComponentModel.DataAnnotations;

namespace FindTheBug.WebAPI.Contracts.Requests;

public class CreateRoleRequest
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<ModulePermissionRequest>? ModulePermissions { get; set; }
}

public class ModulePermissionRequest
{
    public required Guid ModuleId { get; set; }
    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanEdit { get; set; } = false;
    public bool CanDelete { get; set; } = false;
}
