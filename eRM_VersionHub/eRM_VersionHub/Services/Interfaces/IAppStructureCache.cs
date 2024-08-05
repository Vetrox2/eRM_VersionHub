using eRM_VersionHub.Dtos;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppStructureCache
    {
        List<AppStructureDto>? GetAppStructure();
        void SetAppStructure(List<AppStructureDto> appStructure);
        void InvalidateAppStructure();
        void UpdateModuleStatus(VersionDto version, bool publish);
    }
}
