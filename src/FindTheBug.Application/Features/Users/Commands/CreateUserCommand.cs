using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Commands;

public record CreateUserCommand(
    string? Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    string? NIDNumber,
    string? Roles,
    bool IsActive,
    bool AllowUserLogin
) : ICommand<User>;
