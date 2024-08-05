using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;


namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<ApiResponse<Permission?>> CreatePermission(Permission permission);
        Task<ApiResponse<List<Permission>>> GetPermissionList(string Username);
        Task<ApiResponse<Permission?>> DeletePermission(Permission permission);
        Task<bool> ValidatePermissions(VersionDto version, MyAppSettings settings, string? userName);
        Task<ApiResponse<AppPermissionDto>> GetAllPermissionList(string username);
    }
}