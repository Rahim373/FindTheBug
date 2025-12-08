using Microsoft.Extensions.DependencyInjection;

namespace FindTheBug.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Add application services here
        // Example: services.AddScoped<ISampleService, SampleService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddScoped<Common.Interfaces.ITemplateRenderService, Common.Services.TemplateRenderService>();

        return services;
    }
}
