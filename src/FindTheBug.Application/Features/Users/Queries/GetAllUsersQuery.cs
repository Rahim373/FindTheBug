using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Queries;

public record GetAllUsersQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<PagedResult<User>>;
