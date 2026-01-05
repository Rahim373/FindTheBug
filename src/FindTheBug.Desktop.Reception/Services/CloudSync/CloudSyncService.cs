using FindTheBug.Desktop.Reception.Data;
using FindTheBug.Desktop.Reception.Dtos;
using FindTheBug.Desktop.Reception.Services.CloudSync.Mappers;
using FindTheBug.Domain.Entities;
using FindTheBug.Desktop.Reception.CusomEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

    public async Task SyncAsync()
    {
        if (_state.IsSyncing())
        {
            _logger.LogWarning("Sync already in progress, skipping");
            return;
        }

        _state.StartSync();
        _logger.LogInformation("Starting Syncing");

        try
        {
            // Pull data from API
            await SyncEntityAsync<User, User>(CloudSyncConstants.UsersEndpoint, SaveUsersToLocal);
            await SyncEntityAsync<Doctor, Doctor>(CloudSyncConstants.DoctorsEndpoint, SaveDoctorsToLocal);
            await SyncEntityAsync<DiagnosticTestDto, DiagnosticTest>(CloudSyncConstants.DiagnosticTestsEndpoint, SaveDiagnosticTestsToLocal);

            // Push unpushed data to API
            await PushLabReceiptsToApiAsync();

            _state.CompleteSync();
            _logger.LogInformation("Completed Syncing");
        }
        catch (Exception ex)
        {
            _state.FailSync();
            _logger.LogError(ex, "Error during sync");
            throw;
        }
    }

    private void SaveDoctorsToLocal(Result<PagedResult<Doctor>> response, ReceptionDbContext dbContext)
    {
        var doctorSpecialityMaps = response.Data.Items.SelectMany(x => x.DoctorSpecialities).Distinct();
        var doctorSpecialities = doctorSpecialityMaps.Select(x => x.DoctorSpeciality).Distinct();
        var doctors = response.Data.Items;

        foreach (var doctor in doctors)
        {
            if (dbContext.Doctors.Any(x => x.Id == doctor.Id))
                dbContext.Doctors.Update(doctor);
            else
                dbContext.Doctors.Add(doctor);
        }

        foreach (var speciality in doctorSpecialities)
        {
            if (dbContext.DoctorSpecialities.Any(x => x.Id == speciality.Id))
                dbContext.DoctorSpecialities.Update(speciality);
            else
                dbContext.DoctorSpecialities.Add(speciality);
        }

        foreach (var mapping in doctorSpecialityMaps)
        {
            if (dbContext.DoctorSpecialityMappings.Any(x => x.Id == mapping.Id))
                dbContext.DoctorSpecialityMappings.Update(mapping);
            else
                dbContext.DoctorSpecialityMappings.Add(mapping);
        }

        dbContext.SaveChangesAsync();
    }

    private void SaveUsersToLocal(Result<PagedResult<User>> response, ReceptionDbContext dbContext)
    {
        var roleModulePermissions = response.Data.Items.SelectMany(x => x.UserRoles.SelectMany(x => x.Role.RoleModulePermissions)).Distinct();
        var modules = roleModulePermissions.Select(x => x.Module);
        var roles = response.Data.Items.SelectMany(x => x.UserRoles.Select(y => y.Role)).Distinct();
        var users = response.Data.Items;
        var usersRoles = response.Data.Items.SelectMany(x => x.UserRoles).Distinct();

        foreach (var user in users)
        {
            if (dbContext.Users.Any(x => x.Id == user.Id))
                dbContext.Users.Update(user);
            else
                dbContext.Users.Add(user);
        }

        foreach (var module in modules)
        {
            if (dbContext.Modules.Any(x => x.Id == module.Id))
                dbContext.Modules.Update(module);
            else
                dbContext.Modules.Add(module);
        }

        foreach (var role in roles)
        {
            if (dbContext.Roles.Any(x => x.Id == role.Id))
                dbContext.Roles.Update(role);
            else
                dbContext.Roles.Add(role);
        }

        foreach (var userRole in usersRoles)
        {
            if (dbContext.UserRoles.Any(x => x.Id == userRole.Id))
                dbContext.UserRoles.Update(userRole);
            else
                dbContext.UserRoles.Add(userRole);
        }

        foreach (var roleModulePermission in roleModulePermissions)
        {
            if (dbContext.RoleModulePermissions.Any(x => x.Id == roleModulePermission.Id))
                dbContext.RoleModulePermissions.Update(roleModulePermission);
            else
                dbContext.RoleModulePermissions.Add(roleModulePermission);
        }

        dbContext.SaveChangesAsync();
    }

    private void SaveDiagnosticTestsToLocal(Result<PagedResult<DiagnosticTestDto>> response, ReceptionDbContext dbContext)
    {
        var diagnosticTests = response.Data.Items;

        foreach (var dto in diagnosticTests)
        {
            var existingTest = dbContext.DiagnosticTests.FirstOrDefault(x => x.Id == dto.Id);
            if (existingTest != null)
            {
                dto.UpdateEntity(existingTest);
                dbContext.DiagnosticTests.Update(existingTest);
            }
            else
            {
                var newTest = dto.ToEntity();
                dbContext.DiagnosticTests.Add(newTest);
            }
        }

        dbContext.SaveChangesAsync();
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
            throw;
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
            case DiagnosticTestDto testDto when entity is DiagnosticTest test:
                testDto.UpdateEntity(test);
                break;
        }
    }

    /// <summary>
    /// Pushes unpushed LabReceipts to the API
    /// </summary>
    private async Task PushLabReceiptsToApiAsync()
    {
        _logger.LogInformation("Pushing LabReceipts to API...");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ReceptionDbContext>();

            // Get all unpushed LabReceipts (IsDirty = true)
            var unpushedReceipts = await dbContext.LabReceipts
                .Where(lr => lr.IsDirty)
                .Include(lr => lr.TestEntries)
                .ToListAsync();

            if (!unpushedReceipts.Any())
            {
                _logger.LogInformation("No unpushed LabReceipts found");
                return;
            }

            _logger.LogInformation("Found {Count} unpushed LabReceipts", unpushedReceipts.Count);

            foreach (var receipt in unpushedReceipts)
            {
                try
                {
                    // Map LabReceipt to DTO
                    var receiptDto = new LabReceiptSyncDto
                    {
                        Id = receipt.Id,
                        InvoiceNumber = receipt.InvoiceNumber,
                        FullName = receipt.FullName,
                        Age = receipt.Age,
                        IsAgeYear = receipt.IsAgeYear,
                        Gender = receipt.Gender,
                        PhoneNumber = receipt.PhoneNumber,
                        Address = receipt.Address,
                        ReferredByDoctorId = receipt.ReferredByDoctorId,
                        SubTotal = receipt.SubTotal,
                        Total = receipt.Total,
                        Discount = receipt.Discount,
                        Due = receipt.Due,
                        Balance = receipt.Balace,
                        ReportDeliveredOn = receipt.ReportDeliveredOn,
                        LabReceiptStatus = (int)receipt.LabReceiptStatus,
                        ReportDeliveryStatus = (int)receipt.ReportDeliveryStatus,
                        CreatedAt = receipt.CreatedAt,
                        UpdatedAt = receipt.UpdatedAt,
                        CreatedBy = receipt.CreatedBy,
                        UpdatedBy = receipt.UpdatedBy,
                        TestEntries = receipt.TestEntries.Select(te => new ReceiptTestSyncDto
                        {
                            Id = te.Id,
                            LabReceiptId = te.LabReceiptId,
                            DiagnosticTestId = te.DiagnosticTestId,
                            Amount = te.Amount,
                            DiscountPercentage = te.DiscountPercentage,
                            Total = te.Total,
                            Status = (int)te.Status,
                            CreatedAt = te.CreatedAt,
                            UpdatedAt = te.UpdatedAt,
                            CreatedBy = te.CreatedBy,
                            UpdatedBy = te.UpdatedBy
                        }).ToList()
                    };

                    // Send to API
                    var response = await _retryPolicy.ExecuteAsync(async () =>
                        await _httpClient.PostAsJsonAsync(CloudSyncConstants.LabReceiptsEndpoint, receiptDto, _jsonOptions));

                    response.EnsureSuccessStatusCode();

                    // Mark as not dirty after successful sync
                    receipt.IsDirty = false;
                    dbContext.LabReceipts.Update(receipt);

                    _logger.LogInformation("Successfully pushed LabReceipt with InvoiceNumber: {InvoiceNumber}", receipt.InvoiceNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to push LabReceipt with InvoiceNumber: {InvoiceNumber}", receipt.InvoiceNumber);
                    // Continue with next receipt even if this one fails
                }
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation("Completed pushing LabReceipts to API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pushing LabReceipts to API");
            throw;
        }
    }
}