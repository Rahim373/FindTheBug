using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.DTOs;

namespace FindTheBug.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<UserResponseDto>;
