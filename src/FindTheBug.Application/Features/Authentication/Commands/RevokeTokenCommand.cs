using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record RevokeTokenCommand(string RefreshToken) : ICommand<bool>;
