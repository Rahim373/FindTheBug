using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Users.Commands;

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
) : ICommand<UserResponseDto>;
