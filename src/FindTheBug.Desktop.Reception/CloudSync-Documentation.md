# Cloud Synchronization Documentation

This document describes the cloud synchronization feature that syncs RBAC (Role-Based Access Control) data from the cloud API to the local SQLite database in FindTheBug.Desktop.Reception.

## Overview

The cloud sync feature runs as a background service that automatically synchronizes RBAC data from the cloud API to the local database every 1 minute. This ensures that the desktop application always has up-to-date user, role, module, and permission data from the cloud.

## Architecture

### Components

1. **CloudSyncService** (`Services/CloudSync/CloudSyncService.cs`)
   - Main service that handles synchronization logic
   - Uses generic `SyncEntityAsync<TDto, TEntity>` method for all entity types
   - Makes HTTP requests to cloud API endpoints with Polly retry mechanism
   - Updates local SQLite database with synced data
   - Maintains sync state via `SyncState` property

2. **SyncTimerService** (`Services/CloudSync/SyncTimerService.cs`)
   - Background service that runs on a timer
   - Syncs data every 1 minute
   - Performs initial sync on application startup

3. **DTOs** (`Services/CloudSync/Dtos/SyncDtos.cs`)
   - Data transfer objects for API responses
   - Includes: UserDto, RoleDto, UserRoleDto, ModuleDto, RoleModulePermissionDto

4. **Mappers** (`Services/CloudSync/Mappers/SyncMappers.cs`)
   - Extension methods for mapping DTOs to entities
   - `ToEntity()` - Creates new entity from DTO
   - `UpdateEntity()` - Updates existing entity from DTO

5. **Constants** (`Services/CloudSync/CloudSyncConstants.cs`)
   - Centralized API endpoint constants
   - Easy to modify and maintain API URLs

6. **SyncState** (`Services/CloudSync/SyncState.cs`)
   - Observable state object for sync status
   - Implements INotifyPropertyChanged for WPF binding
   - Tracks: IsSyncing, LastSyncSuccess, LastSyncTime, ErrorMessage
   - Provides user-friendly status text

7. **SyncStatusColorConverter** (`Converters/SyncStatusColorConverter.cs`)
   - WPF value converter for sync status indicator
   - Returns: Yellow (syncing), Red (error), LimeGreen (success), Gray (never synced)

8. **Configuration** (`appsettings.json`)
   - API base URL configuration
   - Connection string for local database

## Synchronized Data

The following RBAC entities are synchronized from the cloud:

### 1. Users
- User accounts with basic information
- Email, first name, last name
- Active/inactive status
- Timestamps (CreatedAt, UpdatedAt)

**API Endpoint**: `GET /api/sync/users`

**Mapping Notes**:
- `UserName` from API is not mapped (not in User entity)
- `PasswordHash` and `Phone` are required fields set to empty strings for synced users

### 2. Roles
- User roles (e.g., Admin, Doctor, Receptionist)
- Role descriptions
- Active/inactive status

**API Endpoint**: `GET /api/sync/roles`

### 3. User Roles
- Many-to-many relationship between users and roles
- Assignment timestamps

**API Endpoint**: `GET /api/sync/user-roles`

**Mapping Notes**:
- `CreatedAt` from API maps to `AssignedAt` in User entity

### 4. Modules
- Application modules (e.g., Patients, Doctors, Billing, Laboratory)
- Module descriptions and display names

**API Endpoint**: `GET /api/sync/modules`

**Mapping Notes**:
- `Key` from API maps to `DisplayName` in Module entity

### 5. Role Module Permissions
- Granular permissions for each role per module
- Permissions: View, Create, Edit, Delete
- Note: `CanApprove` from API is ignored (not in RoleModulePermission entity)

**API Endpoint**: `GET /api/sync/role-module-permissions`

## Sync Logic

The sync process follows this logic for each entity type:

1. **Fetch Data** - Make HTTP GET request to cloud API endpoint with Polly retry
2. **Validate Response** - Check for success and data availability
3. **Identify Differences** - Compare existing local IDs with incoming remote IDs
4. **Remove Deleted** - Delete local records not in remote data
5. **Add New** - Insert new records from remote data
6. **Update Modified** - Update existing records with changes
7. **Save Changes** - Persist all changes to local database
8. **Update State** - Update SyncState with current status

### Generic Sync Method

The implementation uses a generic `SyncEntityAsync<TDto, TEntity>` method that works with any entity type:

```csharp
private async Task SyncEntityAsync<TDto, TEntity>(string endpoint)
    where TEntity : class, new()
    where TDto : class
{
    var entityName = typeof(TEntity).Name;
    _logger.LogInformation("Syncing {EntityName}...", entityName);

    try
    {
        // Fetch data from API with Polly retry
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync(endpoint));

        response.EnsureSuccessStatusCode();

        var syncResponse = await response.Content.ReadFromJsonAsync<SyncResponse<TDto>>(_jsonOptions);

        if (syncResponse?.Data == null || !syncResponse.Success)
        {
            _logger.LogWarning("Failed to sync {EntityName}: {Message}", entityName, syncResponse?.Message);
            return;
        }

        // Get existing IDs and compare with incoming data
        // Add new entities, update existing ones, remove deleted ones
        // Save changes to database

        await dbContext.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error syncing {EntityName}", entityName);
        // Don't throw - allow other entity types to sync
    }
}
```

### Parallel Sync

All entity types are synchronized in parallel using `Task.WhenAll()`:
```csharp
var tasks = new[]
{
    SyncEntityAsync<UserDto, User>(CloudSyncConstants.UsersEndpoint),
    SyncEntityAsync<RoleDto, Role>(CloudSyncConstants.RolesEndpoint),
    SyncEntityAsync<UserRoleDto, UserRole>(CloudSyncConstants.UserRolesEndpoint),
    SyncEntityAsync<ModuleDto, Module>(CloudSyncConstants.ModulesEndpoint),
    SyncEntityAsync<RoleModulePermissionDto, RoleModulePermission>(CloudSyncConstants.RoleModulePermissionsEndpoint)
};

await Task.WhenAll(tasks);
```

### Polly Retry Mechanism

HTTP requests use Polly for automatic retry with exponential backoff:

```csharp
_retryPolicy = Policy
    .Handle<HttpRequestException>()
    .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timeSpan, retryCount, context) =>
        {
            _logger.LogWarning(
                "Retry {RetryCount} after {Delay}s due to: {Reason}",
                retryCount, timeSpan.TotalSeconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
        });
```

**Retry Configuration**:
- 3 retry attempts
- Exponential backoff: 2s, 4s, 8s
- Retries on: HttpRequestException and non-success HTTP responses
- Logs warning on each retry

### Sync State Management

The `SyncState` class tracks sync status and provides WPF binding:

**Properties**:
- `IsSyncing` - Currently syncing
- `LastSyncSuccess` - Last sync was successful
- `LastSyncTime` - Time of last sync attempt
- `ErrorMessage` - Error message from failed sync
- `HasError` - Whether there is an error
- `SyncStatusText` - User-friendly status text
- `LastSyncTimeFormatted` - Formatted last sync time

**Methods**:
- `StartSync()` - Marks sync as in progress
- `CompleteSync()` - Marks sync as successful
- `FailSync(string message)` - Marks sync as failed with error message
- `Reset()` - Resets all state to initial values

**UI Integration**:
- `MainWindowViewModel` exposes sync state properties
- `MainWindow.xaml` binds sync status and color indicator
- Properties update automatically via INotifyPropertyChanged

## Configuration

### Desktop Reception appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=reception.db"
  },
  "ApiSettings": {
    "BaseUrl": "http://localhost:5231",
    "SyncClientKey": "desktop-sync-client",
    "SyncClientSecret": "your-secret-key-here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### WebAPI appsettings.json

```json
{
  "ApiSettings": {
    "SyncClientKey": "desktop-sync-client",
    "SyncClientSecret": "your-secret-key-here"
  }
}
```

### API Base URL

Configure the cloud API base URL in Desktop Reception `appsettings.json`:
- **Development**: `http://localhost:5231`
- **Production**: Update to your production API URL

### Basic Authentication Credentials

The sync endpoints are protected using Basic Authentication. Configure matching credentials:

**Desktop Reception** (`appsettings.json`):
- `SyncClientKey`: Client identifier (e.g., "desktop-sync-client")
- `SyncClientSecret`: Secret key for authentication

**WebAPI** (`appsettings.json`):
- `SyncClientKey`: Must match desktop client key
- `SyncClientSecret`: Must match desktop client secret

**Important**: Ensure both `SyncClientKey` and `SyncClientSecret` are identical in both Desktop Reception and WebAPI appsettings.json files.

## Service Registration

Services are registered in `App.xaml.cs`:

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // ... other services ...

    // Register HttpClient for cloud sync
    services.AddHttpClient<CloudSyncService>();

    // Register cloud sync services
    services.AddScoped<CloudSyncService>();
    services.AddHostedService<SyncTimerService>();

    // ... DbContext and other services
}
```

## Lifecycle

### Application Startup
1. Build host with background services
2. Start background services
3. Apply database migrations
4. Initial sync runs immediately

### Scheduled Sync
- Every 1 minute
- Automatic retry on failure
- Logs all sync activities

### Application Shutdown
- Background services are stopped gracefully
- All pending operations complete
- Resources are disposed

## Logging

Sync activities are logged with the following levels:

**Information**:
- Sync timer started/stopped
- Initial sync completed
- Scheduled sync started/completed
- Records synced counts

**Warning**:
- Failed to sync specific entity type
- API returned non-success response

**Error**:
- Exceptions during sync process
- HTTP request failures
- Database save failures

Example log entries:
```
[14:30:00 INF] Sync Timer Service started. Sync interval: 00:01:00
[14:30:00 INF] Starting RBAC sync from cloud...
[14:30:00 INF] Syncing Modules...
[14:30:01 INF] Synced 5 modules
[14:30:01 INF] Syncing Roles...
[14:30:01 INF] Synced 3 roles
[14:30:01 INF] Syncing Users...
[14:30:01 INF] Synced 10 users
[14:30:01 INF] Syncing UserRoles...
[14:30:01 INF] Synced 15 user-roles
[14:30:02 INF] Syncing RoleModulePermissions...
[14:30:02 INF] Synced 25 role-module-permissions
[14:30:02 INF] RBAC sync completed successfully
[14:30:02 INF] Initial sync completed
[14:31:02 INF] Starting scheduled sync...
[14:31:02 INF] RBAC sync completed successfully
```

## API Response Format

Expected API response format:

```json
{
  "success": true,
  "message": "Sync successful",
  "data": [
    {
      "id": "guid-here",
      // ... entity properties
    }
  ],
  "serverTime": "2025-12-26T14:30:00Z"
}
```

## Error Handling

The sync service includes comprehensive error handling:

1. **HTTP Errors**
   - Failed requests are logged
   - Exception is thrown and caught by timer
   - Retry occurs in next sync cycle

2. **API Errors**
   - Non-success responses are logged as warnings
   - Partial sync continues (one entity type failure doesn't stop others)
   - Sync continues in next cycle

3. **Database Errors**
   - Save failures are logged
   - Transaction is rolled back automatically by EF Core
   - Retry occurs in next sync cycle

## Troubleshooting

### Sync Not Running

**Symptoms**: No sync logs appearing

**Solutions**:
1. Check `appsettings.json` has `ApiSettings:BaseUrl`
2. Verify cloud API is accessible
3. Check logs for service startup errors
4. Ensure `SyncTimerService` is registered in `ConfigureServices`

### Connection Failures

**Symptoms**: Connection refused or timeout errors

**Solutions**:
1. Verify API base URL is correct
2. Check network connectivity
3. Ensure cloud API is running
4. Check firewall rules

### Database Lock Issues

**Symptoms**: Database save failures

**Solutions**:
1. Ensure no other processes access the database
2. Close any database viewing tools (DB Browser for SQLite)
3. Restart application to release locks

### Out of Sync Data

**Symptoms**: Local data doesn't match cloud

**Solutions**:
1. Check API response data integrity
2. Verify sync interval (default: 1 minute)
3. Check logs for failed sync attempts
4. Manually trigger sync by restarting application

## Customization

### Changing Sync Interval

Modify `SyncTimerService.cs`:

```csharp
private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(5); // Change to 5 minutes
```

### Adding New Entity Types

1. Add DTO in `SyncDtos.cs`
2. Add sync method in `CloudSyncService.cs`
3. Add to sync tasks in `SyncAsync()` method
4. Implement endpoint in cloud API

### Disabling Automatic Sync

Comment out or remove in `App.xaml.cs`:

```csharp
// services.AddHostedService<SyncTimerService>();
```

Then manually trigger sync when needed:

```csharp
var syncService = serviceProvider.GetRequiredService<CloudSyncService>();
await syncService.SyncAsync();
```

## Security Considerations

1. **HTTPS in Production**
   - Change `BaseUrl` to HTTPS in production
   - Configure SSL certificates

2. **Basic Authentication**
   - Sync endpoints are protected using Basic Authentication
   - Client credentials are stored in appsettings.json
   - Ensure credentials are kept secure and not committed to version control
   - Use strong, unique secrets in production environments
   - Consider using secrets management in production (Azure Key Vault, AWS Secrets Manager, etc.)

3. **API Authentication**
   - Basic Authentication is configured on DataSyncController
   - Credentials are validated against appsettings.json configuration
   - Failed authentication returns 401 Unauthorized status

4. **Data Validation**
   - Validate API responses before processing
   - Sanitize input data

## Performance

- **Initial Sync**: ~5-10 seconds (parallel sync)
- **Scheduled Sync**: ~2-5 seconds (smaller updates)
- **Network Traffic**: Depends on data size
- **Database Impact**: Minimal (uses bulk operations)

## Next Steps

1. Add API endpoints in WebAPI for each sync method
2. Apply `[BasicAuth]` attribute to DataSyncController or individual sync endpoints
3. Add sync status indicator in UI
4. Implement manual sync trigger button
5. Add sync error notifications to users
6. Implement offline mode with cached data

### WebAPI Controller Setup

Create a DataSyncController in WebAPI with the following structure:

```csharp
using FindTheBug.WebAPI.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FindTheBug.WebAPI.Controllers;

[ApiController]
[Route("api/sync")]
[BasicAuth] // Apply Basic Authentication to all endpoints in this controller
public class DataSyncController : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        // Return all users with proper format
        var response = new
        {
            success = true,
            message = "Sync successful",
            data = users.ToList(),
            serverTime = DateTime.UtcNow
        };
        return Ok(response);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        // Return all roles
    }

    [HttpGet("user-roles")]
    public async Task<IActionResult> GetUserRoles()
    {
        // Return all user-roles
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
    {
        // Return all modules
    }

    [HttpGet("role-module-permissions")]
    public async Task<IActionResult> GetRoleModulePermissions()
    {
        // Return all role-module-permissions
    }
}
```

**Note**: The `[BasicAuth]` attribute validates the Authorization header against the configured `SyncClientKey` and `SyncClientSecret` in WebAPI appsettings.json.