using FindTheBug.Application.Features.Roles.Commands;
using FindTheBug.WebAPI.Contracts.Requests;
using Mapster;

namespace FindTheBug.WebAPI.Mappings;

public class RoleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map CreateRoleRequest to CreateRoleCommand
        config.NewConfig<CreateRoleRequest, CreateRoleCommand>()
            .Map(dest => dest.ModulePermissions,
                src => src.ModulePermissions!= null
                    ? src.ModulePermissions.Select(mp => new ModulePermissionDto(
                        mp.ModuleId,
                        mp.CanView,
                        mp.CanCreate,
                        mp.CanEdit,
                        mp.CanDelete)).ToList()
                    : null);

        // Map UpdateRoleRequest to UpdateRoleCommand
        config.NewConfig<UpdateRoleRequest, UpdateRoleCommand>()
            .Map(dest => dest.Id, src => Guid.Empty); // Will be overridden by controller
    }
}
