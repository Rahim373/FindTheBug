using FindTheBug.Application.Features.TestParameters.Commands;
using FindTheBug.WebAPI.Contracts.Requests;
using Mapster;

namespace FindTheBug.WebAPI.Mappings;

public class TestParameterMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map UpdateTestParameterRequest to UpdateTestParameterCommand
        config.NewConfig<UpdateTestParameterRequest, UpdateTestParameterCommand>()
            .Map(dest => dest.Id, src => Guid.Empty); // Will be overridden by controller
    }
}
