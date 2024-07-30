using eRM_VersionHub.Dtos;
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

        public async Task<bool> ValidatePermissions(VersionDto version, MyAppSettings settings, string? userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            var response = await repository.GetPermissionList(userName);
            if (!response.Success || response.Data == null || response.Data.Count == 0)
                return false;

            List<string> modulesList = [];
            foreach (var appPerm in response.Data)
            {
                var appModel = AppDataScanner.GetAppJsonModel(Path.Combine(settings.AppsPath, appPerm.AppID, settings.ApplicationConfigFile));
                if (appModel != null)
                    modulesList.AddRange(appModel.Modules.Select(module => module.ModuleId));
            }

            foreach (var module in version.Modules)
                if (!modulesList.Contains(module.Name))
                    return false;

            return true;
        }
    }
}