using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FindTheBug.Desktop.Reception.Services.CloudSync;
using Microsoft.Extensions.DependencyInjection;

namespace FindTheBug.Desktop.Reception.Converters;

/// <summary>
/// Converts SyncState properties to colors for the sync status indicator
/// </summary>
public class SyncStatusColorConverter : IValueConverter
{
    private readonly IServiceProvider? _serviceProvider;

    public SyncStatusColorConverter()
    {
        // Try to get service provider from App
        if (App.ServiceProvider != null)
        {
            _serviceProvider = App.ServiceProvider;
        }
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (_serviceProvider == null)
        {
            return Colors.Gray;
        }

        var syncState = _serviceProvider.GetService<SyncState>();
        if (syncState == null)
        {
            return Colors.Gray;
        }

        // Priority order: IsSyncing > HasError > LastSyncSuccess
        if (syncState.IsSyncing)
        {
            return Colors.Yellow; // Sync in progress
        }
        else if (syncState.HasError)
        {
            return Colors.Red; // Sync failed
        }
        else if (syncState.LastSyncSuccess && syncState.LastSyncTime.HasValue)
        {
            return Colors.LimeGreen; // Sync successful
        }
        else if (!syncState.LastSyncTime.HasValue)
        {
            return Colors.Gray; // Never synced
        }

        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}