using FindTheBug.Infrastructure.Persistence;

namespace FindTheBug.WebAPI.Extensions;

public static class WebApplicationExtensions
{
    public static async Task UseDatabaseInitializer(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
        await initializer.InitializeAsync();
    }
}
