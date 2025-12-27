namespace FindTheBug.Desktop.Reception.Services.CloudSync;

/// <summary>
/// Constants for cloud synchronization API endpoints
/// </summary>
public static class CloudSyncConstants
{
    /// <summary>
    /// Base path for all sync endpoints
    /// </summary>
    public const string ApiBasePath = "/api/service-sync";
    /// <summary>
    /// Endpoint for syncing doctors
    /// </summary>
    public const string DoctorsEndpoint = $"{ApiBasePath}/doctors";
    /// <summary>
    /// Endpoint for syncing users
    /// </summary>
    public const string UsersEndpoint = $"{ApiBasePath}/users";
    /// <summary>
    /// Endpoint for health check
    /// </summary>
    public const string HealthCheckEndpoint = $"/health";
}