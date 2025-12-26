using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using FindTheBug.Desktop.Reception.Data;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Services.CloudSync.Mappers;
using FindTheBug.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace FindTheBug.Desktop.Reception.Services.CloudSync;

/// <summary>
/// Service for synchronizing RBAC data from cloud API to local database
/// </summary>
public class CloudSyncService
{
    private readonly HttpClient _httpClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CloudSyncService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly SyncState _state;


    public CloudSyncService(
        HttpClient httpClient,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<CloudSyncService> logger,
        SyncState state)
    {
        _httpClient = httpClient;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
        _state = state;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);

        // Configure Basic Authentication
        var clientKey = _configuration["ApiSettings:SyncClientKey"];
        var clientSecret = _configuration["ApiSettings:SyncClientSecret"];

        if (!string.IsNullOrEmpty(clientKey) && !string.IsNullOrEmpty(clientSecret))
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientKey}:{clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }

        // Configure Polly retry policy: 3 retries with exponential backoff
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
    }

    /// <summary>
    /// Synchronizes all RBAC data from cloud to local database
    /// </summary>
    public async Task SyncAsync()
    {
        await HealthCheckAsync();

        if (_state.IsSyncing)
        {
            _logger.LogWarning("Sync already in progress, skipping");
            return;
        }

        _state.StartSync();
        _logger.LogInformation("Starting RBAC sync from cloud...");

        try
        {
            var tasks = new[]
            {
                SyncEntityAsync<User, User>(CloudSyncConstants.UsersEndpoint, SaveUsersToLocal),
                SyncEntityAsync<ModuleDto, Module>(CloudSyncConstants.ModulesEndpoint)
            };

            await Task.WhenAll(tasks);

            _state.CompleteSync();
            _logger.LogInformation("RBAC sync completed successfully");
        }
        catch (Exception ex)
        {
            _state.FailSync(ex.Message);
            _logger.LogError(ex, "Error during RBAC sync");
            throw;
        }
    }

    private void SaveUsersToLocal(Result<PagedResult<User>> response, ReceptionDbContext dbContext)
    {
        foreach (var user in response.Data.Items)
        {
            var exists = dbContext.Users.Any(x => x.Id == user.Id);

            dbContext.Entry(user).State = exists ? EntityState.Modified : EntityState.Added;
        }
    }

    /// <summary>
    /// Performs a health check on the cloud API to verify connectivity
    /// </summary>
    private async Task HealthCheckAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(CloudSyncConstants.HealthCheckEndpoint);
            response.EnsureSuccessStatusCode();
            _state.IsCloudOnline = true;
            _logger.LogInformation("Cloud API is online");
        }
        catch (Exception ex)
        {
            _state.IsCloudOnline = false;
            _logger.LogWarning(ex, "Cloud API is offline or unreachable");
        }
    }

    /// <summary>
    /// Generic method to synchronize any entity type from cloud API
    /// </summary>
    /// <typeparam name="TDto">DTO type for API response</typeparam>
    /// <typeparam name="TEntity">Entity type for database</typeparam>
    /// <param name="endpoint">API endpoint to fetch data from</param>
    private async Task SyncEntityAsync<TDto, TEntity>(string endpoint, Action<Result<PagedResult<TDto>>, ReceptionDbContext>? customMapping = null)
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

            var syncResponse = await response.Content.ReadFromJsonAsync<Result<PagedResult<TDto>>>(_jsonOptions);

            if (syncResponse?.Data == null || !syncResponse.IsSuccess)
            {
                _logger.LogWarning("Failed to sync {EntityName}: {Message}", entityName, syncResponse!.ErrorMessage);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReceptionDbContext>();

            if (customMapping is not null)
            {
                customMapping?.Invoke(syncResponse, dbContext);
                return;
            }

            var dbSet = dbContext.Set<TEntity>();
            var idProperty = typeof(TEntity).GetProperty("Id");

            if (idProperty == null)
            {
                _logger.LogError("Entity {EntityName} does not have an Id property", entityName);
                return;
            }
            else
            {

                // Get existing IDs
                var existingEntities = await dbSet.ToListAsync();
                var existingIds = existingEntities
                    .Select(e => idProperty.GetValue(e))
                    .OfType<Guid>()
                    .ToHashSet();

                var incomingIds = syncResponse.Data.Items
                    .Select(d => d.GetType().GetProperty("Id")?.GetValue(d))
                    .OfType<Guid>()
                    .ToHashSet();

                // Remove entities not in sync data
                var toRemove = existingIds.Except(incomingIds).ToList();
                if (toRemove.Any())
                {
                    var entitiesToRemove = existingEntities
                        .Where(e => toRemove.Contains((Guid)idProperty.GetValue(e)!))
                        .ToList();

                    dbSet.RemoveRange(entitiesToRemove);
                    _logger.LogDebug("Removed {Count} {EntityName} not in sync data", toRemove.Count, entityName);
                }

                // Add or update entities
                var addedCount = 0;
                var updatedCount = 0;

                foreach (var dto in syncResponse.Data.Items)
                {
                    var dtoId = (Guid?)dto.GetType().GetProperty("Id")?.GetValue(dto);
                    var existingEntity = await dbSet.FindAsync(dtoId);

                    if (existingEntity == null)
                    {
                        // Map DTO to entity and add
                        var newEntity = MapDtoToEntity<TDto, TEntity>(dto);
                        if (newEntity != null)
                        {
                            dbSet.Add(newEntity);
                            addedCount++;
                        }
                    }
                    else
                    {
                        // Update existing entity
                        MapDtoToEntity<TDto, TEntity>(dto, existingEntity);
                        updatedCount++;
                    }
                }
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing {EntityName}", entityName);
            // Don't throw - allow other entity types to sync
        }
    }

    /// <summary>
    /// Maps a DTO to a new entity instance using mappers
    /// </summary>
    private TEntity? MapDtoToEntity<TDto, TEntity>(TDto dto)
        where TEntity : class, new()
        where TDto : class
    {
        return dto switch
        {
            UserDto userDto when typeof(TEntity) == typeof(User) => userDto.ToEntity() as TEntity,
            RoleDto roleDto when typeof(TEntity) == typeof(Role) => roleDto.ToEntity() as TEntity,
            UserRoleDto userRoleDto when typeof(TEntity) == typeof(UserRole) => userRoleDto.ToEntity() as TEntity,
            ModuleDto moduleDto when typeof(TEntity) == typeof(Module) => moduleDto.ToEntity() as TEntity,
            RoleModulePermissionDto permissionDto when typeof(TEntity) == typeof(RoleModulePermission) => permissionDto.ToEntity() as TEntity,
            _ => null
        };
    }

    /// <summary>
    /// Maps a DTO to an existing entity using mappers
    /// </summary>
    private void MapDtoToEntity<TDto, TEntity>(TDto dto, TEntity entity)
        where TEntity : class
        where TDto : class
    {
        switch (dto)
        {
            case UserDto userDto when entity is User user:
                userDto.UpdateEntity(user);
                break;
            case RoleDto roleDto when entity is Role role:
                roleDto.UpdateEntity(role);
                break;
            case UserRoleDto userRoleDto when entity is UserRole userRole:
                userRoleDto.UpdateEntity(userRole);
                break;
            case ModuleDto moduleDto when entity is Module module:
                moduleDto.UpdateEntity(module);
                break;
            case RoleModulePermissionDto permissionDto when entity is RoleModulePermission permission:
                permissionDto.UpdateEntity(permission);
                break;
        }
    }
}