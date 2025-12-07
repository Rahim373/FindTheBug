using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Commands;

public record CreateRoleCommand(
    string Name,
    string? Description,
    bool IsActive,
    List<ModulePermissionDto>? ModulePermissions = null
) : ICommand<Role>;
