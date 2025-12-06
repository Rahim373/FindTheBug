using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Roles.Queries;

public record GetAllRolesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null
) : IQuery<PagedResult<Role>>;
