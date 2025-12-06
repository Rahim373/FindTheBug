using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Roles.Commands;

public record DeleteRoleCommand(Guid Id) : ICommand<bool>;
