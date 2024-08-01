using eRM_VersionHub.Dtos;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace eRM_VersionHub.Services
{
    public class AppStructureCache(IMemoryCache memoryCache, IServiceProvider serviceProvider) : IAppStructureCache
    {
        private readonly string key = "appStructure";

        public Task<List<AppStructureDto>?> GetAppStructureAsync()
        {
            memoryCache.TryGetValue(key, out List<AppStructureDto> appStructure);
            return Task.FromResult(appStructure);
        }

        public Task SetAppStructureAsync(List<AppStructureDto> appStructure)
        {
            memoryCache.Set(key, appStructure, new MemoryCacheEntryOptions{ SlidingExpiration = TimeSpan.FromMinutes(15) });
            return Task.CompletedTask;
        }

        public Task InvalidateAppStructureAsync()
        {
            memoryCache.Remove(key);
            return Task.CompletedTask;
        }

        public async Task UpdateModuleStatus(VersionDto version, bool isPublishAction)
        {
            var appStructure = await GetAppStructureAsync();
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
            }

            SetAppStructureAsync(appStructure);
        }
    }
}
