using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ILogger<PermissionRepository> _logger;
        private readonly IDbRepository _dbRepository;

        public PermissionRepository(
            IDbRepository dbRepository,
            ILogger<PermissionRepository> logger
        )
        {
            _logger = logger;
            _dbRepository = dbRepository;
        }

        public async Task<Permission> CreatePermission(Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Repository,
                "Invoked CreatePermission with data: {permission}",
                permission
            );
            var result = await _dbRepository.EditData<Permission>(
                "INSERT INTO permissions(username, app_id) VALUES (@Username, @AppID) RETURNING *",
                permission
            );
            if (result == null)
            {
                throw new InvalidOperationException("Failed to create permission");
            }
            _logger.LogInformation(
                AppLogEvents.Repository,
                "CreatePermission completed successfully"
            );
            return result;
        }

        public async Task<List<Permission>> GetPermissionList(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Repository,
                "Invoked GetPermissionList with parameter: {Username}",
                Username
            );
            var result = await _dbRepository.GetAll<Permission>(
                "SELECT * FROM permissions WHERE username=@Username",
                new { Username }
            );
            if (result == null || !result.Any())
            {
                throw new NotFoundException($"No permissions found for user {Username}");
            }
            _logger.LogInformation(
                AppLogEvents.Repository,
                "GetPermissionList completed successfully"
            );
            return result;
        }

        public async Task<Permission> DeletePermission(Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Repository,
                "Invoked DeletePermission with data: {permission}",
                permission
            );
            var result = await _dbRepository.EditData<Permission>(
                "DELETE FROM permissions WHERE username=@Username AND app_id=@AppID RETURNING *",
                permission
            );
            if (result == null)
            {
                throw new NotFoundException("Permission not found");
            }
            _logger.LogInformation(
                AppLogEvents.Repository,
                "DeletePermission completed successfully"
            );
            return result;
        }
    }
}
