using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string? Email,
    string FirstName,
    string LastName,
    string? Phone,
    string? NIDNumber,
    string? Roles,
    bool IsActive,
    string? Password
) : ICommand<User>;
