namespace FindTheBug.Application.Features.UserManagement.Modules.DTOs;

public record ModuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
