using eRM_VersionHub.Dtos;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace eRM_VersionHub.Services
{
    public class AppStructureCache(IMemoryCache memoryCache) : IAppStructureCache
    {
        private readonly string key = "appStructure";
        public Task<List<AppStructureDto>?> GetAppStructureAsync()
        {
            memoryCache.TryGetValue(key, out List<AppStructureDto> appStructure);
            return Task.FromResult(appStructure);
        }

        public Task SetAppStructureAsync(List<AppStructureDto> appStructure)
        {
            memoryCache.Set(key, appStructure, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        public Task InvalidateAppStructureAsync()
        {
            memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
