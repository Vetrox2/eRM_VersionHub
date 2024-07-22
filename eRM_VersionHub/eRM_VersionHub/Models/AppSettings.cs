using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class AppSettings
    {
        public LoggingSettings Logging { get; set; }
        public string AllowedHosts { get; set; }
        public MyAppSettings MyAppSettings { get; set; }
    }

    public class LoggingSettings
    {
        public LogLevelSettings LogLevel { get; set; }
    }

    public class LogLevelSettings
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    public class MyAppSettings
    {
        public required string ConnectionString { get; set; }
        public required string AppsPath { get; set; }
        public required string ApplicationConfigFile { get; set; }
        public required string InternalPackagesPath { get; set; }
        public required string ExternalPackagesPath { get; set; }
        public override string ToString() => JsonManager.Serialize<MyAppSettings>(this);
    }
}
