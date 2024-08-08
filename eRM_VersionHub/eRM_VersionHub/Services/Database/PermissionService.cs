using eRM_VersionHub.Dtos;
using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repository;
        private readonly ILogger<PermissionService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;

        public PermissionService(
            IPermissionRepository repository,
            ILogger<PermissionService> logger,
            IUserRepository userRepository,
            IServiceProvider serviceProvider
        )
        {
            _repository = repository;
            _logger = logger;
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<Permission> CreatePermission(Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked CreatePermission with data: {permission}",
                permission
            );
            var result = await _repository.CreatePermission(permission);
            if (result == null)
            {
                throw new BadRequestException("Failed to create permission");
            }
            _logger.LogInformation(
                AppLogEvents.Service,
                "CreatePermission returned: {result}",
                result
            );
            return result;
        }

        public async Task<List<Permission>> GetPermissionList(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked GetPermissionList with parameter: {Username}",
                Username
            );
            var result = await _repository.GetPermissionList(Username);
            if (result == null || !result.Any())
            {
                throw new NotFoundException($"No permissions found for user: {Username}");
            }
            _logger.LogInformation(
                AppLogEvents.Service,
                "GetPermissionList returned: {result}",
                result
            );
            return result;
        }

        public async Task<AppPermissionDto> GetAllPermissionList(string username)
        {
            var appDataScanner = _serviceProvider.GetRequiredService<IAppDataScanner>();
            var apps = appDataScanner.GetAppsNames();
            var targetUser = await _userRepository.GetUser(username);

            if (targetUser == null || apps == null)
            {
                throw new InvalidOperationException("Internal server error");
            }

            var userPermissionList = await _repository.GetPermissionList(targetUser.Username);
            var permissionMap = new Dictionary<string, bool>();
            foreach (var item in apps)
            {
                permissionMap[item] = userPermissionList.Any(p => p.AppID == item);
            }

            return new AppPermissionDto
            {
                User = targetUser.Username,
                AppsPermission = permissionMap
            };
        }

        public async Task<Permission> DeletePermission(Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked DeletePermission with data: {permission}",
                permission
            );
            var result = await _repository.DeletePermission(permission);
            if (result == null)
            {
                throw new NotFoundException("Permission not found or could not be deleted");
            }
            _logger.LogInformation(
                AppLogEvents.Service,
                "DeletePermission returned: {result}",
                result
            );
            return result;
        }

        public async Task<bool> ValidatePermissions(
            VersionDto version,
            MyAppSettings settings,
            string? userName
        )
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            var permissions = await _repository.GetPermissionList(userName);
            if (permissions == null || permissions.Count == 0)
                return false;

            List<string> modulesList = [];
            foreach (var appPerm in permissions)
            {
                var appModel = AppDataScanner.GetAppJsonModel(
                    Path.Combine(settings.AppsPath, appPerm.AppID, settings.ApplicationConfigFile)
                );
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
