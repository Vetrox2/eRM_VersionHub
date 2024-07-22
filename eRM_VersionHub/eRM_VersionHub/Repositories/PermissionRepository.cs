using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class PermissionRepository(IDbRepository dbRepository, ILogger<PermissionRepository> logger) : IPermissionRepository
    {
        private readonly ILogger<PermissionRepository> _logger = logger;
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<ApiResponse<Permission?>> CreatePermission(Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked CreatePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await _dbRepository.EditData<Permission>(
                "INSERT INTO permissions(username, app_id) VALUES (@Username, @AppID) RETURNING *", permission);
            _logger.LogInformation(AppLogEvents.Repository, "CreatePermission returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<Permission>>> GetPermissionList(string Username)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetPermissionList with parameter: {Username}", Username);
            ApiResponse<List<Permission>> result = await _dbRepository.GetAll<Permission>(
                "SELECT * FROM permissions WHERE username=@Username", new { Username });
            _logger.LogInformation(AppLogEvents.Repository, "GetPermissionList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<Permission?>> DeletePermission(Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked DeletePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await _dbRepository.EditData<Permission>(
                "DELETE FROM permissions WHERE username=@Username AND app_id=@AppID RETURNING *", permission);
            _logger.LogInformation(AppLogEvents.Repository, "DeletePermission returned: {result}", result);
            return result;
        }
    }
}