using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FindTheBug.Desktop.Reception.Services.CloudSync;

/// <summary>
/// Background service that periodically syncs RBAC data from cloud
/// </summary>
public class SyncTimerService : BackgroundService
{
    private readonly CloudSyncService _cloudSyncService;
    private readonly ILogger<SyncTimerService> _logger;
    private readonly TimeSpan _syncInterval = TimeSpan.FromMinutes(1);

    public SyncTimerService(
        CloudSyncService cloudSyncService,
        ILogger<SyncTimerService> logger)
    {
        _cloudSyncService = cloudSyncService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sync Timer Service started. Sync interval: {Interval}", _syncInterval);

        // Perform initial sync
        try
        {
            await _cloudSyncService.SyncAsync();
            _logger.LogInformation("Initial sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during initial sync");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_syncInterval, stoppingToken);

                _logger.LogInformation("Starting scheduled sync...");
                await _cloudSyncService.SyncAsync();
                _logger.LogInformation("Scheduled sync completed");
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Sync timer service stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during scheduled sync. Will retry in next interval.");
            }
        }

        _logger.LogInformation("Sync Timer Service stopped");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sync Timer Service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}