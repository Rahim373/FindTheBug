using FindTheBug.Application;
using FindTheBug.Infrastructure;

namespace FindTheBug.WebAPI.Installers;

public class LayerInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
    }
}
