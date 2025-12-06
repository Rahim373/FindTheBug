using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Commands;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : ICommand<Role>;
