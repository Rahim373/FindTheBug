using FindTheBug.Application.Features.Users.Commands;
using FindTheBug.WebAPI.Contracts.Requests;
using Mapster;

namespace FindTheBug.WebAPI.Mappings;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map CreateUserRequest to CreateUserCommand
        config.NewConfig<CreateUserRequest, CreateUserCommand>();

        // Map UpdateUserRequest to UpdateUserCommand  
        config.NewConfig<UpdateUserRequest, UpdateUserCommand>()
            .Map(dest => dest.Id, src => Guid.Empty); // Will be overridden by controller
    }
}
