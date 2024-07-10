using eRM_VersionHub.Models;


namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<ApiResponse<Permission?>> CreatePermission(Permission permission);
        Task<ApiResponse<List<Permission>>> GetPermissionList(string Username);
        Task<ApiResponse<Permission?>> DeletePermission(Permission permission);
    }
}