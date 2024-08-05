using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Services.Database
{

    public class PermissionService(IPermissionRepository repository, ILogger<PermissionService> logger, IUserRepository userRepository, IServiceProvider serviceProvider) : IPermissionService
    {
        public async Task<ApiResponse<Permission?>> CreatePermission(Permission permission)
        {
            logger.LogDebug(AppLogEvents.Service, "Invoked CreatePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await repository.CreatePermission(permission);

            logger.LogInformation(AppLogEvents.Service, "CreatePermission returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<Permission>>> GetPermissionList(string Username)
        {
            logger.LogDebug(AppLogEvents.Service, "Invoked GetPermissionList with parameter: {Username}", Username);
            ApiResponse<List<Permission>> result = await repository.GetPermissionList(Username);

            logger.LogInformation(AppLogEvents.Service, "GetPermissionList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<AppPermissionDto>> GetAllPermissionList(string username)
        {
            var appDataScanner = serviceProvider.GetRequiredService<IAppDataScanner>();
            var apps = appDataScanner.GetAppsNames();
            var targerUser = await userRepository.GetUser(username);
            if (targerUser.Data == null || apps == null)
            {
                var s = ApiResponse<string>.ErrorResponse(["Internal server error"]);

            }
            var userPermissionList = await repository.GetPermissionList(targerUser.Data.Username);
            var permissionMap = new Dictionary<string, bool>();
            foreach (var item in apps.Data)
            {
                permissionMap[item] = userPermissionList.Data.Any(p => p.AppID == item);
            }

            return ApiResponse<AppPermissionDto>.SuccessResponse(new AppPermissionDto() { User = targerUser.Data.Username, AppsPermission = permissionMap });
        }


        public async Task<ApiResponse<Permission?>> DeletePermission(Permission permission)
        {
            logger.LogDebug(AppLogEvents.Service, "Invoked DeletePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await repository.DeletePermission(permission);

            logger.LogInformation(AppLogEvents.Service, "DeletePermission returned: {result}", result);
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