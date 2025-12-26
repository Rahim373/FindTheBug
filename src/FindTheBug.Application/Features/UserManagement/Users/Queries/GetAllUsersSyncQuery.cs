using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Common.Models;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.UserManagement.Users.Queries;

public record GetAllUsersSyncQuery(
    int PageNumber,
    int PageSize
) : IQuery<PagedResult<User>>;
