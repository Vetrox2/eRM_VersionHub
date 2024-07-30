using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services
{
    public class AppDataScanner(IFavoriteService favoriteService, IPermissionService permissionService, ILogger<AppDataScanner> logger) : IAppDataScanner
    {
        private readonly IFavoriteService _favoriteService = favoriteService;
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ILogger<AppDataScanner> _logger = logger;
        private ApiResponse<List<AppStructureDto>> response;

        public static ApplicationJsonModel? GetAppJsonModel(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
                return null;

            string jsonString = File.ReadAllText(jsonFilePath);
            return jsonString.Deserialize<ApplicationJsonModel>();
        }

        /// <summary>
        /// Return list with every name from modulesNames, if module doesn't exist or is empty than Versions will be an empty list.
        /// </summary>
        public static List<ModuleModel> GetModuleModels(string path, List<string> modulesNames)
        {
            var modules = new List<ModuleModel>();

            foreach (var moduleName in modulesNames)
            {
                string moduleFilePath = Path.Combine(path, moduleName);
                var versionsNames = GetDirectoryInfo(moduleFilePath);

                var versions = new List<string>();
                versionsNames?.ForEach(version => { versions.Add(version.Name); });

                modules.Add(new ModuleModel() { Name = moduleName, Versions = versions });
            }

            return modules;
        }

        public static List<DirectoryInfo>? GetDirectoryInfo(string path)
        {
            if (!Directory.Exists(path))
                return null;

            var info = new DirectoryInfo(path);
            return info.GetDirectories()?.ToList();
        }

        public async Task<ApiResponse<List<AppStructureDto>>> GetAppsStructure(MyAppSettings settings, string userToken)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking GetAppsStructure with paramters: {userToken}, {settings}", userToken, settings);
            response = new();

            if (!Directory.Exists(settings.InternalPackagesPath) || !Directory.Exists(settings.ExternalPackagesPath) || !Directory.Exists(settings.AppsPath))
            {
                _logger.LogError(AppLogEvents.Service, "At least one of the path in the settings does not exist");
                response.Errors.Add("Fatal error");
                return response;
            }

            var structure = await GetInternalAppStructure(settings.AppsPath, settings.ApplicationConfigFile, settings.InternalPackagesPath, userToken);
            _logger.LogDebug(AppLogEvents.Service, "GetInternalAppStructure returned: {structure}", structure);

            if (structure == null)
            {
                _logger.LogError(AppLogEvents.Service, "Internal app structure is null");
                return response;
            }

            structure = await SetFavorites(structure, userToken);
            _logger.LogDebug(AppLogEvents.Service, "SetFavorites returned: {structure}", structure);

            structure = SetPublished(settings.ExternalPackagesPath, structure);
            _logger.LogDebug(AppLogEvents.Service, "SetPublished returned: {structure}", structure);

            response.Data = structure;
            _logger.LogInformation(AppLogEvents.Service, "GetAppsStructure returned: {response}", response);
            return response;
        }

        public ApiResponse<List<string>> GetAppsNames(MyAppSettings settings)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking GetAppsNames");

            if (!Directory.Exists(settings.AppsPath))
            {
                _logger.LogError(AppLogEvents.Service, "AppsPath in the settings does not exist");
                return ApiResponse<List<string>>.ErrorResponse(["Fatal error"]);
            }

            var info = GetDirectoryInfo(settings.AppsPath);
            var appsNames = info?.Select(x => x.Name).ToList();

            _logger.LogDebug(AppLogEvents.Service, "GetAppsNames returned: {appsNames}", appsNames);

            return appsNames == null ? 
                ApiResponse<List<string>>.ErrorResponse(["Fatal error"]) : ApiResponse<List<string>>.SuccessResponse(appsNames);
        }

        private async Task<List<AppStructureDto>?> GetInternalAppStructure(string appsPath, string appJsonName, string internalPackagesPath, string token)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking GetInternalAppStructure with paramters: {appsPath}, {appJsonName}, {internalPackagesPath}, {token}", appsPath, appJsonName, internalPackagesPath, token);
            var appsStructure = new List<AppStructureDto>();

            var perms = await _permissionService.GetPermissionList(token);
            _logger.LogDebug(AppLogEvents.Service, "GetPermissionList returned: {perms}", perms);

            var appsNames = GetDirectoryInfo(appsPath);
            _logger.LogDebug(AppLogEvents.Service, "GetDirectoryInfo returned: {perms}", perms);

            if (appsNames == null)
            {
                _logger.LogWarning(AppLogEvents.Service, "Applications data not found");
                response.Errors.Add("Applications data not found");
                return null;
            }

            if (perms == null || !perms.Success || perms.Data.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Service, "User permissions not found");
                response.Errors.Add("User permissions not found");
                return null;
            }

            foreach (var app in appsNames)
            {
                var appJSModel = GetAppJsonModel(Path.Combine(appsPath, app.Name, appJsonName));
                _logger.LogDebug(AppLogEvents.Service, "GetAppJsonModel returned: {appJSModel}", appJSModel);

                if (appJSModel == null || !perms.Data.Any(perm => perm.AppID == appJSModel.UniqueIdentifier))
                {
                    _logger.LogWarning(AppLogEvents.Service, "Application not found or user has no permissions for it: {appName}", app.Name);
                    continue;
                }

                var moduleModels = GetModuleModels(internalPackagesPath, appJSModel.GetModulesNames());
                _logger.LogDebug(AppLogEvents.Service, "GetModuleModels returned: {moduleModels}", moduleModels);

                var appStructureDto = CreateAppStructureDto(appJSModel, moduleModels);
                _logger.LogDebug(AppLogEvents.Service, "CreateAppStructureDto returned: {appStructureDto}", appStructureDto);

                if (appStructureDto == null)
                {
                    _logger.LogWarning(AppLogEvents.Service, "Something went wrong during data creation for the application: {appName}", appJSModel.Name);
                    response.Errors.Add($"Something went wrong during data creation for the application: {appJSModel.Name}");
                    continue;
                }

                _logger.LogDebug(AppLogEvents.Service, "Adding application to the list: {appName}", appJSModel.Name);
                appsStructure.Add(appStructureDto);
            }

            _logger.LogInformation(AppLogEvents.Service, "GetInternalAppStructure returned: {appsStructure}", appsStructure);
            return appsStructure;
        }

        private List<AppStructureDto> SetPublished(string externalPackagesPath, List<AppStructureDto> appsStructure)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking SetPublished with paramters: {externalPackagesPath}, {appsStructure}", externalPackagesPath, appsStructure);
            foreach (var app in appsStructure)
            {
                foreach (var appVersion in app.Versions)
                {
                    var modulesUnderVersion = appVersion.Modules.Select(module => module.Name).ToList();
                    _logger.LogDebug(AppLogEvents.Service, "Modules under version: {modulesUnderVersion}", modulesUnderVersion);

                    var publishedModules = GetModuleModels(externalPackagesPath, modulesUnderVersion);
                    _logger.LogDebug(AppLogEvents.Service, "GetModuleModels returned: {publishedModules}", publishedModules);

                    foreach (var module in appVersion.Modules)
                    {
                        var publishedModule = publishedModules.FirstOrDefault(m => m.Name == module.Name);
                        _logger.LogDebug(AppLogEvents.Service, "Published module: {publishedModule}", publishedModule);

                        var publishedVersion = publishedModule?.Versions.FirstOrDefault(version => TagService.CompareVersions(version, appVersion.ID));
                        _logger.LogDebug(AppLogEvents.Service, "Published version: {publishedVersion}", publishedVersion);

                        if (!string.IsNullOrEmpty(publishedVersion))
                        {
                            _logger.LogDebug(AppLogEvents.Service, "Setting module as published");
                            module.IsPublished = true;
                            appVersion.PublishedTag = TagService.GetTag(publishedVersion);
                        }
                    }
                }
            }

            _logger.LogInformation(AppLogEvents.Service, "SetPublished returned: {appsStructure}", appsStructure);
            return appsStructure;
        }

        private async Task<List<AppStructureDto>> SetFavorites(List<AppStructureDto> appsStructure, string token)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking SetFavorites with paramters: {appsStructure}, {token}", appsStructure, token);
            var favs = await _favoriteService.GetFavoriteList(token);
            _logger.LogDebug(AppLogEvents.Service, "GetFavoriteList returned: {favs}", favs);

            if (favs != null && favs.Success && favs.Data.Count > 0)
            {
                _logger.LogDebug(AppLogEvents.Service, "Setting favorites for the applications");
                appsStructure.ForEach(app => app.IsFavourite = favs.Data.Any(fav => fav.AppID == app.ID));
            }

            _logger.LogInformation(AppLogEvents.Service, "SetFavorites returned: {appsStructure}", appsStructure);
            return appsStructure;
        }

        private AppStructureDto? CreateAppStructureDto(ApplicationJsonModel appJSModel, List<ModuleModel> moduleModels)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoking CreateAppStructureDto with paramters: {appJSModel}, {moduleModels}", appJSModel, moduleModels);
            var appStructureDto = new AppStructureDto() { ID = appJSModel.UniqueIdentifier, Name = appJSModel.Name, Versions = [] };

            var mainModuleID = appJSModel.Modules.FindIndex(module => module.Optional == false);
            _logger.LogDebug(AppLogEvents.Service, "Main module ID: {mainModuleID}", mainModuleID);

            //If there is no non-optional module, we consider the first optional module as the determinant of the application version
            if (mainModuleID == -1)
            {
                _logger.LogWarning(AppLogEvents.Service, "No main module found, considering the first optional module as the main module");
                mainModuleID = 0;
            }

            var appMainModule = moduleModels.FirstOrDefault(module => module.Name == appJSModel.Modules[mainModuleID].ModuleId);
            _logger.LogDebug(AppLogEvents.Service, "Main module: {appMainModule}", appMainModule);

            //If there is no module that determines the version of the application, or it has no versions, the application will not be in the inventory
            if (appMainModule == null || appMainModule.Versions == null || appMainModule.Versions.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Service, "No module that determines the version of the application, or it has no versions");
                return null;
            }

            foreach (var version in appMainModule.Versions)
            {
                var existingModules = appJSModel.Modules.Where(module =>
                    moduleModels.Any(moduleModel => module.ModuleId == moduleModel.Name && moduleModel.Versions.Contains(version)));
                _logger.LogDebug(AppLogEvents.Service, "Existing modules for the version {version}: {existingModules}", version, existingModules);

                var moduleDtos = existingModules.Select(module =>
                    new ModuleDto() { Name = module.ModuleId, IsOptional = module.Optional }).ToList();
                _logger.LogDebug(AppLogEvents.Service, "Modules for the version {version}: {moduleDtos}", version, moduleDtos);

                var versionDto = new VersionDto(version, moduleDtos);
                _logger.LogDebug(AppLogEvents.Service, "Adding version to the application: {versionDto}", versionDto);
                appStructureDto.Versions.Add(versionDto);
            }

            _logger.LogInformation(AppLogEvents.Service, "CreateAppStructureDto returned: {appStructureDto}", appStructureDto);
            return appStructureDto;
        }
    }
}
