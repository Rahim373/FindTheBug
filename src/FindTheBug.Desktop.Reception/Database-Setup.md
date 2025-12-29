# Database Setup Guide - FindTheBug.Desktop.Reception

This document explains how the Entity Framework Core with SQLite database is set up in the FindTheBug.Desktop.Reception application.

## Overview

The desktop reception application uses **Entity Framework Core with SQLite** as the database provider, following a **Code-First** approach. The database is automatically created when the application starts.

## Configuration

### 1. Connection String

The database connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=reception.db"
  }
}
```

You can modify the connection string to:
- Change the database file name: `Data Source=mydatabase.db`
- Use a full path: `Data Source=C:\Data\reception.db`
- Enable WAL mode for better concurrency: `Data Source=reception.db;Cache=Shared`

### 2. NuGet Packages

The following NuGet packages have been added:
- `Microsoft.EntityFrameworkCore.Sqlite` (10.0.1) - SQLite database provider
- `Microsoft.EntityFrameworkCore.Design` (10.0.1) - Design-time tools for EF Core
- `Microsoft.Extensions.Configuration` (10.0.1) - Configuration support
- `Microsoft.Extensions.Configuration.Json` (10.0.1) - JSON configuration provider
- `Microsoft.Extensions.DependencyInjection` (10.0.1) - Dependency injection

## DbContext

The `ReceptionDbContext` class is located in `Data/ReceptionDbContext.cs` and includes:

### Entities

- **Patient Management**
  - `Patients` - Patient information
  
- **Doctors Management**
  - `Doctors` - Doctor profiles
  - `DoctorSpecialities` - Medical specialties
  - `DoctorSpecialityMappings` - Many-to-many relationship between doctors and specialties
  
- **Invoice/Receipt Management**
  - `Invoices` - Invoice/receipt records
  - `InvoiceItems` - Individual items within invoices

### Features

- **Automatic Timestamps**: `CreatedAt` and `UpdatedAt` are automatically set
- **Relationships**: Proper foreign key relationships with cascade delete rules
- **Data Validation**: Required fields and length constraints
- **Precision**: Decimal fields configured with proper precision (18, 2)

## Usage

### Accessing the DbContext

The DbContext is registered in the dependency injection container in `App.xaml.cs`:

```csharp
public static IServiceProvider? ServiceProvider { get; private set; }

protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    var serviceCollection = new ServiceCollection();
    ConfigureServices(serviceCollection);
    
    ServiceProvider = serviceCollection.BuildServiceProvider();
    
    // Database is automatically created on startup
    var dbContext = ServiceProvider.GetRequiredService<ReceptionDbContext>();
    dbContext.Database.EnsureCreated();
}
```

### Using DbContext in ViewModels

To use the DbContext in your ViewModels, inject it through the service provider:

```csharp
public class ExampleViewModel
{
    private readonly ReceptionDbContext _dbContext;
    
    public ExampleViewModel(ReceptionDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task LoadPatientsAsync()
    {
        var patients = await _dbContext.Patients
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ToListAsync();
    }
}
```

### Example: Adding a New Patient

```csharp
var patient = new Patient
{
    PatientCode = "P001",
    FirstName = "John",
    LastName = "Doe",
    MobileNumber = "+1234567890",
    Email = "john.doe@example.com",
    Address = "123 Main Street",
    IsActive = true
};

_dbContext.Patients.Add(patient);
await _dbContext.SaveChangesAsync();
```

### Example: Querying with Relationships

```csharp
var invoicesWithPatients = await _dbContext.Invoices
    .Include(i => i.Patient)
    .Include(i => i.InvoiceItems)
    .Where(i => i.Status == "Issued")
    .ToListAsync();
```

## Database File Location

The SQLite database file (`reception.db`) is created in the application's output directory:
- **Debug**: `src\FindTheBug.Desktop.Reception\bin\Debug\net10.0-windows\reception.db`
- **Release**: `src\FindTheBug.Desktop.Reception\bin\Release\net10.0-windows\reception.db`

## Migration Strategy

The application uses **EF Core Migrations** for database schema management. This is the recommended approach for production scenarios.

### Design-Time Factory

A `ReceptionDbContextFactory` class is provided to support design-time operations (migrations):

```csharp
public class ReceptionDbContextFactory : IDesignTimeDbContextFactory<ReceptionDbContext>
{
    public ReceptionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReceptionDbContext>();
        optionsBuilder.UseSqlite("Data Source=reception_design.db");
        return new ReceptionDbContext(optionsBuilder.Options);
    }
}
```

### Creating New Migrations

When you make changes to your entities or DbContext, create a new migration:

```bash
# Add a new migration (replace AddNewFeature with descriptive name)
dotnet ef migrations add AddInitialMigration --project src\FindTheBug.Desktop.Reception --startup-project src\FindTheBug.Desktop.Reception

# Remove the last migration (if not applied)
dotnet ef migrations remove --project src\FindTheBug.Desktop.Reception

# List all migrations
dotnet ef migrations list --project src\FindTheBug.Desktop.Reception
```

### Applying Migrations

Migrations are automatically applied on application startup in `App.xaml.cs`:

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    // ... other setup code ...
    
    // Apply migrations to ensure database is up-to-date
    var dbContext = ServiceProvider.GetRequiredService<ReceptionDbContext>();
    dbContext.Database.Migrate();
}
```

This ensures that the database schema is always synchronized with your entity definitions.

## Best Practices

1. **Always use `async` methods** for database operations to prevent UI freezing
2. **Dispose the DbContext properly** - Use `using` statements or dependency injection
3. **Track changes carefully** - EF Core automatically tracks entities loaded from the database
4. **Use `Include()` for relationships** - Explicitly load navigation properties when needed
5. **Handle exceptions** - Wrap database operations in try-catch blocks

## Troubleshooting

### Database file locked error
- Ensure only one instance of the application is running
- Check that no other tools (like DB Browser for SQLite) have the file open

### Connection string not found
- Verify `appsettings.json` is in the output directory
- Check that the file is set to "Copy to Output Directory: PreserveNewest"

### Build errors
- Ensure the `FindTheBug.Domain` project reference is added
- Restore NuGet packages: `dotnet restore`

## Viewing the Database

You can view and edit the SQLite database using tools like:
- **DB Browser for SQLite** (https://sqlitebrowser.org/)
- **SQLiteStudio** (https://sqlitestudio.pl/)
- **Visual Studio** (Server Explorer → Data Connections)

## Next Steps

1. Add repositories for better data access abstraction
2. Implement unit of work pattern for complex transactions
3. Add data seeding for initial data
4. Set up migrations for production deployment
5. Add logging for database operations