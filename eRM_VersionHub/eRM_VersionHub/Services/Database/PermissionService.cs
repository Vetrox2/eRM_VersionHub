using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class PermissionService(IPermissionRepository repository, ILogger<PermissionService> logger) : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger = logger;
        private readonly IPermissionRepository _repository = repository;

        public async Task<ApiResponse<Permission?>> CreatePermission(Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked CreatePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await _repository.CreatePermission(permission);

            _logger.LogInformation(AppLogEvents.Service, "CreatePermission returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<Permission>>> GetPermissionList(string Username)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetPermissionList with parameter: {Username}", Username);
            ApiResponse<List<Permission>> result = await _repository.GetPermissionList(Username);

            _logger.LogInformation(AppLogEvents.Service, "GetPermissionList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<Permission?>> DeletePermission(Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked DeletePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await _repository.DeletePermission(permission);

            _logger.LogInformation(AppLogEvents.Service, "DeletePermission returned: {result}", result);
            return result;
        }
    }
}