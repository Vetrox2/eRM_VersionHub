using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using Microsoft.AspNetCore.Mvc.Testing;

namespace eRM_VersionHub_Tester.Helpers
{
    public class TestFixture : WebApplicationFactory<Program>, IDisposable
    {
        private static readonly string _connectionString = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres;";
        private string _tempDir;
        private string _appSettingsPath;

        public TestFixture()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);

            _appSettingsPath = Path.Combine(_tempDir, "appsettings.Test.json");
        }

        public void SetNewAppSettings(string appJson, string appsPath, string internalPath, string externalPath)
        {
            var settings = new AppSettings();
            settings.MyAppSettings = new MyAppSettings
            {
                ApplicationConfigFile = appJson,
                ConnectionString = _connectionString,
                AppsPath = appsPath,
                InternalPackagesPath = internalPath,
                ExternalPackagesPath = externalPath
            };

            File.WriteAllText(_appSettingsPath, settings.Serialize());
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Remove the existing configuration options
                config.Sources.Clear();

                config.AddJsonFile(_appSettingsPath, optional: true, reloadOnChange: true);

                // Add any additional configuration here if necessary
                config.AddEnvironmentVariables();
            });

            builder.UseEnvironment("Testing");
            base.ConfigureWebHost(builder);
        }

        public void Dispose()
        {
            Directory.Delete(_tempDir, true);
        }

    }
}