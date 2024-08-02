using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Services
{
    public class FileChangeWatcher : BackgroundService
    {
        private readonly FileSystemWatcher _watcher;
        private readonly IAppStructureCache _cache;
        private readonly MyAppSettings _settings;
        private readonly ILogger<FileChangeWatcher> _logger;

        public FileChangeWatcher(IAppStructureCache cache, IOptions<AppSettings> appSettings, ILogger<FileChangeWatcher> logger)
        {
            _settings = appSettings.Value.MyAppSettings;
            _watcher = new FileSystemWatcher
            {
                Path = _settings.InternalPackagesPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnRenamed;
            _cache = cache;
            _logger = logger;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation(AppLogEvents.Service, $"File {e.ChangeType}: {e.FullPath}");
            _cache.InvalidateAppStructure();
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _logger.LogInformation(AppLogEvents.Service, $"File renamed from {e.OldFullPath} to {e.FullPath}");
            _cache.InvalidateAppStructure();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(AppLogEvents.Service, "FileChangeWatcher is starting.");
            _watcher.EnableRaisingEvents = true;

            stoppingToken.Register(() =>
            {
                _logger.LogInformation(AppLogEvents.Service,"FileChangeWatcher is stopping.");
                _watcher.EnableRaisingEvents = false;
            });

            // Return a completed task since we're using the FileSystemWatcher events
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _watcher?.Dispose();
            base.Dispose();
        }
    }

}
