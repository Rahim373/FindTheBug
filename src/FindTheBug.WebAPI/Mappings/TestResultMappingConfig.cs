using FindTheBug.Application.Features.TestResults.Commands;
using FindTheBug.WebAPI.Contracts.Requests;
using Mapster;

namespace FindTheBug.WebAPI.Mappings;

public class TestResultMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Map UpdateTestResultRequest to UpdateTestResultCommand
        config.NewConfig<UpdateTestResultRequest, UpdateTestResultCommand>()
            .Map(dest => dest.Id, src => Guid.Empty); // Will be overridden by controller

        // Map VerifyRequest to VerifyTestResultsCommand  
        config.NewConfig<VerifyRequest, VerifyTestResultsCommand>()
            .Map(dest => dest.TestEntryId, src => Guid.Empty); // Will be overridden by controller (Id parameter)
    }
}
