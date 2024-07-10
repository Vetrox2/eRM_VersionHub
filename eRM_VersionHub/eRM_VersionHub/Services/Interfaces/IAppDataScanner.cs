using eRM_VersionHub.Dtos;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppDataScanner
    {
        Task<List<AppStructureDto>> GetAppsStructure(string appsPath, string appJsonName, string internalPackagesPath, string externalPackagesPath, string userToken);
    }
}
