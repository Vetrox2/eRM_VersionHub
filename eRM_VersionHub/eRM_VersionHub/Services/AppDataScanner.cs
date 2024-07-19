using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using System.Text.Json;
using eRM_VersionHub.Services.Interfaces;
using System.ComponentModel;


namespace eRM_VersionHub.Services
{
    public class AppDataScanner(IFavoriteService favoriteService, IPermissionService permissionService) : IAppDataScanner
    {
        private IFavoriteService _favoriteService = favoriteService;
        private IPermissionService _permissionService = permissionService;

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

        public async Task<List<AppStructureDto>?> GetAppsStructure(MyAppSettings settings, string userToken)
        {
            var structure = await GetInternalAppStructure(settings.AppsPath, settings.ApplicationConfigFile, settings.InternalPackagesPath, userToken);
            if (structure == null)
                return null;

            structure = await SetFavorites(structure, userToken);
            structure = SetPublished(settings.ExternalPackagesPath, structure);
            return structure;
        }

        private async Task<List<AppStructureDto>?> GetInternalAppStructure(string appsPath, string appJsonName, string internalPackagesPath, string token)
        {
            var perms = await _permissionService.GetPermissionList(token);
            var appsStructure = new List<AppStructureDto>();
            var appsNames = GetDirectoryInfo(appsPath);

            if(appsNames == null || perms == null || !perms.Success || perms.Data.Count == 0) 
                return null;

            foreach (var app in appsNames)
            {
                var appJSModel = GetAppJsonModel(Path.Combine(appsPath, app.Name, appJsonName));

                if (appJSModel == null || !perms.Data.Any(perm => perm.AppID == appJSModel.UniqueIdentifier))
                    continue;

                var moduleModels = GetModuleModels(internalPackagesPath, appJSModel.GetModulesNames());
                var appStructureDto = CreateAppStructureDto(appJSModel, moduleModels);

                if (appStructureDto == null)
                    continue;

                appsStructure.Add(appStructureDto);
            }

            return appsStructure;
        }

        private List<AppStructureDto> SetPublished(string externalPackagesPath, List<AppStructureDto> appsStructure)
        {
            foreach (var app in appsStructure)
            {
                foreach (var appVersion in app.Versions)
                {
                    var modulesUnderVersion = appVersion.Modules.Select(module => module.Name).ToList();
                    var publishedModules = GetModuleModels(externalPackagesPath, modulesUnderVersion);

                    foreach (var module in appVersion.Modules)
                    {
                        var publishedModule = publishedModules.FirstOrDefault(m => m.Name == module.Name);
                        var publishedVersion = publishedModule?.Versions.FirstOrDefault(version => TagService.CompareVersions(version, appVersion.ID));
                        if (!string.IsNullOrEmpty(publishedVersion))
                        {
                            module.IsPublished = true;
                            appVersion.PublishedTag = TagService.GetTag(publishedVersion);
                        }
                    }
                }
            }

            return appsStructure;
        }

        private async Task<List<AppStructureDto>> SetFavorites(List<AppStructureDto> appsStructure, string token)
        {
            var favs = await _favoriteService.GetFavoriteList(token);

            if (favs != null && favs.Success && favs.Data.Count > 0)
                appsStructure.ForEach(app => app.IsFavourite = favs.Data.Any(fav => fav.AppID == app.ID));

            return appsStructure;
        }

        private AppStructureDto? CreateAppStructureDto(ApplicationJsonModel appJSModel, List<ModuleModel> moduleModels)
        {
            var appStructureDto = new AppStructureDto() { ID = appJSModel.UniqueIdentifier, Name = appJSModel.Name, Versions = [] };

            //If there is no non-optional module, we consider the first optional module as the determinant of the application version
            var mainModuleID = appJSModel.Modules.FindIndex(module => module.Optional == false);
            if (mainModuleID == -1)
                mainModuleID = 0;

            //If there is no module that determines the version of the application, or it has no versions, the application will not be in the inventory
            var appMainModule = moduleModels.FirstOrDefault(module => module.Name == appJSModel.Modules[mainModuleID].ModuleId);
            if (appMainModule == null || appMainModule.Versions == null || appMainModule.Versions.Count == 0)
                return null;

            foreach (var version in appMainModule.Versions)
            {
                var existingModules = appJSModel.Modules.Where(module =>
                    moduleModels.Any(moduleModel => module.ModuleId == moduleModel.Name && moduleModel.Versions.Contains(version)));

                var moduleDtos = existingModules.Select(module =>
                    new ModuleDto() { Name = module.ModuleId, IsOptional = module.Optional }).ToList();

                var versionDto = new VersionDto(version, moduleDtos);
                appStructureDto.Versions.Add(versionDto);
            }

            return appStructureDto;
        }
    }
}
