using eRM_VersionHub.Models;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<Permission?>> CreatePermission(Permission permission);
        Task<Result<List<Permission>>> GetPermissionList(string Username);
        Task<Result<Permission?>> DeletePermission(Permission permission);
    }
}