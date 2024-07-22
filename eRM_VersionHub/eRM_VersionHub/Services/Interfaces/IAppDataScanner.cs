using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppDataScanner
    {
        Task<ApiResponse<List<AppStructureDto>>> GetAppsStructure(MyAppSettings settings, string userToken);
    }
}
