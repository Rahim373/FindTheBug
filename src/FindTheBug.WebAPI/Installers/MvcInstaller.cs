using FindTheBug.WebAPI.Filters;
using FindTheBug.WebAPI.Mappings;

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
        
        // Add Mapster
        services.AddMapster();
    }
}
