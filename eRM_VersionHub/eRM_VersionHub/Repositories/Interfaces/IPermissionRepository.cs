using eRM_VersionHub.Models;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Result<Permission?>> CreatePermission(Permission permission);
        Task<Result<List<Permission>>> GetPermissionList(string Username);
        Task<Result<Permission?>> DeletePermission(Permission permission);
    }
}