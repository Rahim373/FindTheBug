namespace FindTheBug.WebAPI.Installers;

public class SwaggerInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
