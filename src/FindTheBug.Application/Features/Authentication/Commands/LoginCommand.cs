using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Authentication.Contracts;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record LoginCommand(
    string EmailOrPhone,
    string Password
) : ICommand<LoginResponse>;
