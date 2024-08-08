using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Permission> CreatePermission(Permission permission);
        Task<List<Permission>> GetPermissionList(string Username);
        Task<AppPermissionDto> GetAllPermissionList(string username);
        Task<Permission> DeletePermission(Permission permission);
        Task<bool> ValidatePermissions(
            VersionDto version,
            MyAppSettings settings,
            string? userName
        );
    }
}
