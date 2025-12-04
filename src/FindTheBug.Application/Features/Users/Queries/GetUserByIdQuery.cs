using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IQuery<User>;
