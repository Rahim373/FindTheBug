using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.WebAPI.Contracts.Requests;
using Mapster;

namespace FindTheBug.WebAPI.Mappings;

public class RoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Simplified mapping - Mapster will handle the module permissions automatically
        config.NewConfig<CreateRoleRequest, CreateRoleCommand>();
        config.NewConfig<UpdateRoleRequest, UpdateRoleCommand>();
    }
}
