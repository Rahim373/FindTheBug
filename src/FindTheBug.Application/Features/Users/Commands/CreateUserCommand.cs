using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.DTOs;

namespace FindTheBug.Application.Features.Users.Commands;

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
