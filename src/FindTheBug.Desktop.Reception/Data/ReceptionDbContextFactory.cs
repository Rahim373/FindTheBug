using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FindTheBug.Desktop.Reception.Data;

/// <summary>
/// Factory for creating DbContext instances at design time (e.g., for migrations)
/// </summary>
public class ReceptionDbContextFactory : IDesignTimeDbContextFactory<ReceptionDbContext>
{
    public ReceptionDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<ReceptionDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new ReceptionDbContext(optionsBuilder.Options);
    }
}