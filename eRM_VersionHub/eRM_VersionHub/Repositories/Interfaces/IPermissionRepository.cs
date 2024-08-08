using eRM_VersionHub.Models;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission> CreatePermission(Permission permission);
        Task<List<Permission>> GetPermissionList(string Username);
        Task<Permission> DeletePermission(Permission permission);
    }
}
