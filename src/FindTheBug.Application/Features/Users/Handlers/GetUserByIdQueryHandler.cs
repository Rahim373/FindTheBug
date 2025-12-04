using ErrorOr;
using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Application.Common.Messaging;
using FindTheBug.Application.Features.Users.Queries;
using FindTheBug.Domain.Entities;

namespace FindTheBug.Application.Features.Users.Handlers;

public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork) 
    : IQueryHandler<GetUserByIdQuery, User>
{
    public async Task<ErrorOr<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Repository<User>().GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        return user;
    }
}
