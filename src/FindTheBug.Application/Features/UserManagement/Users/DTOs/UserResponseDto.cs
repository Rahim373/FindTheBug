namespace FindTheBug.Application.Features.UserManagement.Users.DTOs;

public record ModulePermissionDto
{
    public string Module { get; init; } = string.Empty;
    public string Permission { get; init; } = string.Empty;
}

public record UserResponseDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Phone { get; init; } = string.Empty;
    public string? NIDNumber { get; init; }
    public bool IsActive { get; init; }
    public bool AllowUserLogin { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public List<UserRoleDto>? Roles { get; init; }
    public List<ModulePermissionDto>? Permissions { get; init; }
}

public record UserListItemDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Phone { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int RoleCount { get; init; }
    public DateTime? LastLoginAt { get; internal set; }
}

public record UserRoleDto
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
}