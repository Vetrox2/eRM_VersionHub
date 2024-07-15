using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using Microsoft.AspNetCore.Mvc.Testing;

namespace eRM_VersionHub_Tester.Tests
{
    public class TestFixture : WebApplicationFactory<Program>
    {
        private static readonly string _connectionString = "Host=localhost;Port=5433;Database=testdb;Username=postgres;Password=postgres_test";
        public static void SetNewAppSettings(string appJson, string appsPath, string internalPath, string externalPath)
        {
            string path = "appsettings.Test.json";
            File.WriteAllText(path, new AppSettings().Serialize());
            var settings = File.ReadAllText(path).Deserialize<AppSettings>();
            settings.MyAppSettings = new();
            settings.MyAppSettings.ApplicationConfigFile = appJson;
            settings.MyAppSettings.ConnectionString = _connectionString;
            settings.MyAppSettings.AppsPath = appsPath;
            settings.MyAppSettings.InternalPackagesPath = internalPath;
            settings.MyAppSettings.ExternalPackagesPath = externalPath;
            File.WriteAllText(path, settings.Serialize());
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                // Remove the existing configuration options
                config.Sources.Clear();
                
                config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),"appsettings.Test.json"), optional: true, reloadOnChange: true);

                // Add any additional configuration here if necessary
                config.AddEnvironmentVariables();
            });

            builder.UseEnvironment("Testing");
            base.ConfigureWebHost(builder);
        }

    }
}