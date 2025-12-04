using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Users.Commands;

public record DeleteUserCommand(Guid Id) : ICommand<bool>;
