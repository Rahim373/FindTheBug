using System.ComponentModel;

namespace FindTheBug.Desktop.Reception.Services.CloudSync;

/// <summary>
/// Represents the current state of cloud synchronization
/// </summary>
public class SyncState : INotifyPropertyChanged
{
    private bool _isSyncing;
    private bool _isCloudOnline;
    private bool _lastSyncSuccess;
    private DateTime? _lastSyncTime;
    private string? _errorMessage;

    /// <summary>
    /// Gets or Sets if cloud is online
    /// </summary>
    public bool IsCloudOnline
    {
        get => _isCloudOnline;
        set {
            if (_isCloudOnline != value)
            {
                _isCloudOnline = value;
            }
            OnPropertyChanged(nameof(IsCloudOnline));
        }
    }

    /// <summary>
    /// Gets or sets whether a sync is currently in progress
    /// </summary>
    public bool IsSyncing
    {
        get => _isSyncing;
        set
        {
            if (_isSyncing != value)
            {
                _isSyncing = value;
                OnPropertyChanged(nameof(IsSyncing));
                OnPropertyChanged(nameof(SyncStatusText));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the last sync was successful
    /// </summary>
    public bool LastSyncSuccess
    {
        get => _lastSyncSuccess;
        set
        {
            if (_lastSyncSuccess != value)
            {
                _lastSyncSuccess = value;
                OnPropertyChanged(nameof(LastSyncSuccess));
                OnPropertyChanged(nameof(SyncStatusText));
            }
        }
    }

    /// <summary>
    /// Gets or sets the time of the last sync attempt
    /// </summary>
    public DateTime? LastSyncTime
    {
        get => _lastSyncTime;
        set
        {
            if (_lastSyncTime != value)
            {
                _lastSyncTime = value;
                OnPropertyChanged(nameof(LastSyncTime));
                OnPropertyChanged(nameof(SyncStatusText));
                OnPropertyChanged(nameof(LastSyncTimeFormatted));
            }
        }
    }

    /// <summary>
    /// Gets or sets the error message from the last failed sync
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(SyncStatusText));
            }
        }
    }

    /// <summary>
    /// Gets whether there is an error from the last sync
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Gets a user-friendly status text for the sync
    /// </summary>
    public string SyncStatusText
    {
        get
        {
            if (IsSyncing)
                return "Syncing...";

            if (!LastSyncTime.HasValue)
                return "Not synced yet";

            if (!LastSyncSuccess)
                return $"Sync failed: {ErrorMessage ?? "Unknown error"}";

            var timeAgo = DateTime.UtcNow - LastSyncTime.Value;
            var minutesAgo = (int)timeAgo.TotalMinutes;
            
            if (minutesAgo < 1)
                return "Synced just now";
            if (minutesAgo < 60)
                return $"Synced {minutesAgo} min ago";

            var hoursAgo = (int)timeAgo.TotalHours;
            if (hoursAgo < 24)
                return $"Synced {hoursAgo} hours ago";

            return $"Synced on {LastSyncTime:yyyy-MM-dd HH:mm}";
        }
    }

    /// <summary>
    /// Gets the last sync time in a formatted string
    /// </summary>
    public string LastSyncTimeFormatted => LastSyncTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never";

    /// <summary>
    /// Resets the sync state
    /// </summary>
    public void Reset()
    {
        IsSyncing = false;
        LastSyncSuccess = false;
        LastSyncTime = null;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the start of a sync operation
    /// </summary>
    public void StartSync()
    {
        IsSyncing = true;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the completion of a successful sync
    /// </summary>
    public void CompleteSync()
    {
        IsSyncing = false;
        LastSyncSuccess = true;
        LastSyncTime = DateTime.UtcNow;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the completion of a failed sync
    /// </summary>
    public void FailSync(string message)
    {
        IsSyncing = false;
        LastSyncSuccess = false;
        LastSyncTime = DateTime.UtcNow;
        ErrorMessage = message;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}