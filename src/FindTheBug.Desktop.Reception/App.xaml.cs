using System.Windows;
using FindTheBug.Common.Services;
using FindTheBug.Desktop.Reception.Data;
using FindTheBug.Desktop.Reception.Services.CloudSync;
using FindTheBug.Desktop.Reception.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FindTheBug.Desktop.Reception
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        private IHost? _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Build host with background services
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

            ServiceProvider = _host.Services;

            // Start background services
            _host.StartAsync();

            // Apply migrations to ensure database is up-to-date
            var dbContext = ServiceProvider.GetRequiredService<ReceptionDbContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.MigrateAsync().GetAwaiter();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Register configuration
            services.AddSingleton(configuration);

            // Register HttpClient for cloud sync
            services.AddHttpClient<CloudSyncService>();

            // Register cloud sync services
            services.AddScoped<CloudSyncService>();
            services.AddSingleton<SyncState>();
            services.AddSingleton<ReportService>();
            services.AddHostedService<SyncTimerService>();

            // Register DbContext with SQLite connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ReceptionDbContext>(options =>
                options.UseSqlite(connectionString));

            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Stop background services
            if (_host != null)
            {
                _host.StopAsync().GetAwaiter().GetResult();
                _host.Dispose();
            }

            // Clean up services
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
}