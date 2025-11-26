using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Contracts;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
