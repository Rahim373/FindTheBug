using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Roles.DTOs;

namespace FindTheBug.Application.Features.Roles.Commands;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : ICommand<RoleResponseDto>;
