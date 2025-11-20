using Microsoft.Extensions.DependencyInjection;

namespace FindTheBug.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add application services here
        // Example: services.AddScoped<ISampleService, SampleService>();
        
        return services;
    }
}
