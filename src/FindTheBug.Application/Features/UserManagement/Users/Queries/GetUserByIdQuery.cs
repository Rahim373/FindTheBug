using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.UserManagement.Users.DTOs;

namespace FindTheBug.Application.Features.UserManagement.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserResponseDto>;
