using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eRM_VersionHub.Services
{
    public class AppStructureCache(IMemoryCache memoryCache, IServiceProvider serviceProvider, ILogger<AppStructureCache> logger) : IAppStructureCache
    {
        private readonly string key = "appStructure";

        public List<AppStructureDto>? GetAppStructure()
        {
            memoryCache.TryGetValue(key, out List<AppStructureDto> appStructure);
            return appStructure;
        }

        public void SetAppStructure(List<AppStructureDto> appStructure)
        {
            logger.LogDebug(AppLogEvents.Service, "Saving new apps structure into cache");
            memoryCache.Set(key, appStructure, new MemoryCacheEntryOptions{ SlidingExpiration = TimeSpan.FromMinutes(15) });
        }

        public void InvalidateAppStructure()
        {
            logger.LogDebug(AppLogEvents.Service, "Deleting apps structure from cache");
            memoryCache.Remove(key);
        }

        public void UpdateModuleStatus(VersionDto version, bool isPublishAction)
        {
            logger.LogDebug(AppLogEvents.Service, "Updating apps structure in cache, version: {version}", version);

            var appStructure = GetAppStructure();
            if (appStructure == null)
            {
                serviceProvider.GetRequiredService<IAppDataScanner>().GetCurrentStructureAndSaveToCache();
                return;
            }

            foreach(var app in appStructure) 
            { 
                var publishedVersion = app.Versions.FirstOrDefault(v => v.ID == version.ID);
                if (publishedVersion == null) 
                    continue;

                bool correctApp = false;
                version.Modules.ForEach(m =>
                {
                    var module = publishedVersion.Modules.FirstOrDefault(module => m.Name == module.Name);
                    if (module != null)
                    {
                        module.IsPublished = isPublishAction;
                        correctApp = true;
                    }
                });

                if(correctApp)
                    publishedVersion.PublishedTag = version.PublishedTag;

                if (publishedVersion.Modules.All(module => !module.IsPublished))
                    publishedVersion.PublishedTag = "";
            }

            SetAppStructure(appStructure);
        }
    }
}
