using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Roles.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Roles.Commands;

public record CreateRoleCommand(
    string Name,
    string? Description,
    bool IsActive,
    List<ModulePermissionDto>? ModulePermissions = null
) : ICommand<RoleResponseDto>;
