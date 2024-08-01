using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IAppDataScanner
    {
        Task<ApiResponse<List<AppStructureDto>>> GetAppsStructure(string userToken);
        ApiResponse<List<string>> GetAppsNames();
    }
}
