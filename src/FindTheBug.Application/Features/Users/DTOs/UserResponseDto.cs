namespace FindTheBug.Application.Features.Users.DTOs;

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
}

public record UserRoleDto
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
}
