using Mapster;
using MapsterMapper;
using System.Reflection;

namespace FindTheBug.WebAPI.Mappings;

public static class MapsterConfiguration
{
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        
        // Scan the WebAPI assembly for mapping configurations
        config.Scan(Assembly.GetExecutingAssembly());
        
        // Register the mapper
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        
        return services;
    }
}
