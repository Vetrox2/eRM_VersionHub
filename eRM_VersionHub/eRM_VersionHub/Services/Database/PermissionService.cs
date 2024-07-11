using eRM_VersionHub.Models;

using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class PermissionService(IPermissionRepository repository) : IPermissionService
    {
        private readonly IPermissionRepository _repository = repository;

        public async Task<ApiResponse<Permission?>> CreatePermission(Permission permission)
        {
            return await _repository.CreatePermission(permission);
        }

        public async Task<ApiResponse<List<Permission>>> GetPermissionList(string Username)
        {
            return await _repository.GetPermissionList(Username);
        }

        public async Task<ApiResponse<Permission?>> DeletePermission(Permission permission)
        {
            return await _repository.DeletePermission(permission);
        }
    }
}