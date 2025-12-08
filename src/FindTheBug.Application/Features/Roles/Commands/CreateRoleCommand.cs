using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.DTOs;

namespace FindTheBug.Application.Features.Roles.Commands;

public record CreateRoleCommand(
    string Name,
    string? Description,
    bool IsActive,
    List<ModulePermissionDto>? ModulePermissions = null
) : ICommand<RoleResponseDto>;
