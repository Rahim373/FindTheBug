using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record ResetPasswordCommand(
    string Token,
    string NewPassword
) : ICommand<bool>;
