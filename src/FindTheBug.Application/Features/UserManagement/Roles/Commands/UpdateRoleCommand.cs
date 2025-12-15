using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Roles.Commands;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : ICommand<RoleResponseDto>;
