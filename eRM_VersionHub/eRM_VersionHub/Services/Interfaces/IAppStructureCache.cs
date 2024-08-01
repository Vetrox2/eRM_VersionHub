using eRM_VersionHub.Dtos;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppStructureCache
    {
        Task<List<AppStructureDto>?> GetAppStructureAsync();
        Task SetAppStructureAsync(List<AppStructureDto> appStructure);
        Task InvalidateAppStructureAsync();
    }
}
