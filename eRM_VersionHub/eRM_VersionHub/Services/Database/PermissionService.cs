using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class PermissionService(IPermissionRepository repository) : IPermissionService
    {
        private readonly IPermissionRepository _repository = repository;

        public async Task<Result<Permission?>> CreatePermission(Permission permission)
        {
            return await _repository.CreatePermission(permission);
        }

        public async Task<Result<List<Permission>>> GetPermissionList(string Username)
        {
            return await _repository.GetPermissionList(Username);
        }

        public async Task<Result<Permission?>> DeletePermission(Permission permission)
        {
            return await _repository.DeletePermission(permission);
        }
    }
}