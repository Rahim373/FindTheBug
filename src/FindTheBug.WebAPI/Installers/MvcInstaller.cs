using FindTheBug.WebAPI.Filters;

namespace FindTheBug.WebAPI.Installers;

public class MvcInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ErrorOrActionFilter>();
        });

        services.AddHttpContextAccessor();
    }
}
