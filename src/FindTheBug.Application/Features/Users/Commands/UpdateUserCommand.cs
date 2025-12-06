using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string? Email,
    string FirstName,
    string LastName,
    string Phone,
    string? NIDNumber,
    List<Guid> RoleIds,
    bool IsActive,
    bool AllowUserLogin,
    string? Password
) : ICommand<User>;
