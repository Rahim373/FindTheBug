using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.UserManagement.Users.Commands;

public record DeleteUserCommand(Guid Id) : ICommand<bool>;
