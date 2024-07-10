using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories.Database
{
    public class PermissionRepository(IDbRepository dbRepository) : IPermissionRepository
    {
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<Result<Permission?>> CreatePermission(Permission permission)
        {
            Result<Permission?> CreatedPermission = await _dbRepository.EditData<Permission>(
                "INSERT INTO permissions(username, app_id) VALUES (@Username, @AppID) RETURNING *", permission);
            return CreatedPermission;
        }

        public async Task<Result<List<Permission>>> GetPermissionList(string Username)
        {
            Result<List<Permission>> PermissionList = await _dbRepository.GetAll<Permission>(
                "SELECT * FROM permissions WHERE username=@Username", new { Username });
            return PermissionList;
        }

        public async Task<Result<Permission?>> DeletePermission(Permission permission)
        {
            Result<Permission?> DeletedPermission = await _dbRepository.EditData<Permission>(
                "DELETE FROM permissions WHERE username=@Username AND app_id=@AppID RETURNING *", permission);
            return DeletedPermission;
        }
    }
}