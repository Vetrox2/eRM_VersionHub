using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppDataScanner
    {
        Task<List<AppStructureDto>> GetAppsStructure(string userToken);
        List<string> GetAppsNames();
        List<AppStructureDto>? GetCurrentStructureAndSaveToCache();
    }
}
