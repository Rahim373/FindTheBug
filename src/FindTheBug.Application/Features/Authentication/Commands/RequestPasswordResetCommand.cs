using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record RequestPasswordResetCommand(string Email) : ICommand<bool>;
