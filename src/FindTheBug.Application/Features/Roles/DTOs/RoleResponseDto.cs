namespace FindTheBug.Application.Features.Roles.DTOs;

public record RoleResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public bool IsSystemRole { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<ModulePermissionDto>? ModulePermissions { get; init; }
}

public record RoleListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public bool IsSystemRole { get; init; }
    public int ModuleCount { get; init; }
}

public record ModulePermissionDto
{
    public Guid ModuleId { get; init; }
    public string ModuleName { get; init; } = string.Empty;
    public bool CanView { get; init; }
    public bool CanCreate { get; init; }
    public bool CanEdit { get; init; }
    public bool CanDelete { get; init; }
}
