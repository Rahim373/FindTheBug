using FindTheBug.Application.Common.Messaging;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Queries;

public record GetActiveRolesQuery : IQuery<IEnumerable<Role>>;
