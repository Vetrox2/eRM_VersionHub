using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class PermissionRepository(IDbRepository dbRepository) : IPermissionRepository
    {
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<ApiResponse<Permission?>> CreatePermission(Permission permission)
        {
            ApiResponse<Permission?> CreatedPermission = await _dbRepository.EditData<Permission>(
                "INSERT INTO permissions(username, app_id) VALUES (@Username, @AppID) RETURNING *", permission);
            return CreatedPermission;
        }

        public async Task<ApiResponse<List<Permission>>> GetPermissionList(string Username)
        {
            ApiResponse<List<Permission>> PermissionList = await _dbRepository.GetAll<Permission>(
                "SELECT * FROM permissions WHERE username=@Username", new { Username });
            return PermissionList;
        }

        public async Task<ApiResponse<Permission?>> DeletePermission(Permission permission)
        {
            ApiResponse<Permission?> DeletedPermission = await _dbRepository.EditData<Permission>(
                "DELETE FROM permissions WHERE username=@Username AND app_id=@AppID RETURNING *", permission);
            return DeletedPermission;
        }
    }
}