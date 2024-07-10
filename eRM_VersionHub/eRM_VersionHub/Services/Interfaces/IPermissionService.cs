using eRM_VersionHub.Models;


namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<ApiResponse<Permission?>> CreatePermission(Permission permission);
        Task<ApiResponse<List<Permission>>> GetPermissionList(string Username);
        Task<ApiResponse<Permission?>> DeletePermission(Permission permission);
    }
}