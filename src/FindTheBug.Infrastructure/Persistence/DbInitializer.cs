using FindTheBug.Application.Common.Interfaces;
using FindTheBug.Domain.Entities;
using FindTheBug.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FindTheBug.Infrastructure.Persistence;

public class DbInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializer> _logger;
    private readonly IConfiguration _configuration;

    public DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations
            if (context.Database.IsNpgsql() && context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed SuperUser
            await SeedSuperUserAsync(context, scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedSuperUserAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        var superUserConfig = _configuration.GetSection("SuperUser");
        var email = superUserConfig["Email"];
        
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("SuperUser email not configured. Skipping seeding.");
            return;
        }

        if (await context.Users.AnyAsync(u => u.Email == email))
        {
            return; // User already exists
        }

        var password = superUserConfig["Password"];
        var firstName = superUserConfig["FirstName"] ?? "Super";
        var lastName = superUserConfig["LastName"] ?? "User";

        if (string.IsNullOrEmpty(password))
        {
            _logger.LogError("SuperUser password not configured.");
            return;
        }

        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher>();
        var passwordHash = passwordHasher.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Roles = "Admin,SuperUser",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "System"
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        _logger.LogInformation("SuperUser seeded successfully.");
    }
}
