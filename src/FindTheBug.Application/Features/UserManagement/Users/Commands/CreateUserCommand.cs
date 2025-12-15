using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Users.Commands;

public record CreateUserCommand(
    string? Email,
    string Password,
    string FirstName,
    string LastName,
    string Phone,
    string? NIDNumber,
    List<Guid> RoleIds,
    bool IsActive,
    bool AllowUserLogin
) : ICommand<UserResponseDto>;
