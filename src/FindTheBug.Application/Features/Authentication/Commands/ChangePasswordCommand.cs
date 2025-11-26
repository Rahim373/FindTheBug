using FindTheBug.Application.Common.Messaging;

namespace FindTheBug.Application.Features.Authentication.Commands;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : ICommand<bool>;
