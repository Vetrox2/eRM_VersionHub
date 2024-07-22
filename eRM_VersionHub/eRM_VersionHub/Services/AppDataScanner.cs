using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;


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

        public async Task<List<AppStructureDto>?> GetAppsStructure(string appsPath, string appJsonName, string internalPackagesPath, string externalPackagesPath, string userToken)
        {
            var structure = GetInternalAppStructure(appsPath, appJsonName, internalPackagesPath);
            structure = SetPublishedStatus(externalPackagesPath, structure);
            structure = await GetFilteredByPermsAndFav(structure, userToken);
            return structure;
        }

        private List<AppStructureDto> GetInternalAppStructure(string appsPath, string appJsonName, string internalPackagesPath)
        {
            var appsStructure = new List<AppStructureDto>();
            var moduleModels = GetModuleModels(internalPackagesPath);
            var appsNames = GetDirectoryInfo(appsPath);

            foreach (var app in appsNames)
            {
                var appJSModel = GetAppJsonModel(Path.Combine(appsPath, app.Name, appJsonName));

                if (appJSModel == null)
                    continue;

                var appStructureDto = CreateAppStructureDto(appJSModel, moduleModels);

                if (appStructureDto == null)
                    continue;

                appsStructure.Add(appStructureDto);
            }

            return appsStructure;
        }

        private List<AppStructureDto> SetPublishedStatus(string externalPackagesPath, List<AppStructureDto> appsStructure)
        {
            var publishedModules = GetModuleModels(externalPackagesPath);

            appsStructure.ForEach(app => app.Versions.ForEach(appVersion => appVersion.Modules.ForEach(module =>
            {
                var publishedModule = publishedModules.FirstOrDefault(m => m.Name == module.Name);
                if (publishedModule != null && publishedModule.Versions.Any(version => version == appVersion.ID))
                    module.IsPublished = true;
            })));

            return appsStructure;
        }

        private async Task<List<AppStructureDto>?> GetFilteredByPermsAndFav(List<AppStructureDto> appsStructure, string token)
        {
            var perms = await _permissionService.GetPermissionList(token);
            var favs = await _favoriteService.GetFavoriteList(token);

            if (perms == null || !perms.Success || perms.Data.Count == 0) return null;

            appsStructure = appsStructure.Where(app => perms.Data.Any(perm => perm.AppID == app.ID)).ToList();

            if (favs != null && favs.Success && favs.Data.Count > 0)
                appsStructure.ForEach(app => app.IsFavourite = favs.Data.Any(fav => fav.AppID == app.ID));

            return appsStructure;
        }

        private List<ModuleModel> GetModuleModels(string path)
        {
            var modules = new List<ModuleModel>();
            var modulesNames = GetDirectoryInfo(path);

            foreach (var module in modulesNames)
            {
                string moduleFilePath = Path.Combine(path, module.Name);
                var versionsInfo = new DirectoryInfo(moduleFilePath);
                var versionsNames = versionsInfo.GetDirectories().ToList();

                var versions = new List<string>();
                versionsNames?.ForEach(version => { versions.Add(version.Name); });

                modules.Add(new ModuleModel() { Name = module.Name, Versions = versions });
            }

            return modules;
        }

        private AppStructureDto? CreateAppStructureDto(ApplicationJsonModel appJSModel, List<ModuleModel> moduleModels)
        {
            var appStructureDto = new AppStructureDto() { ID = appJSModel.UniqueIdentifier, Name = appJSModel.Name, Versions = [] };

            //Jeśli nie ma nieopcjonalnego modułu, uznajemy za wyznacznik wersji aplikacji pierwszy opcjonalny moduł
            var mainModuleID = appJSModel.Modules.FindIndex(module => module.Optional == false);
            if (mainModuleID == -1)
                mainModuleID = 0;

            //Jeśli nie istnieje moduł wyznaczający wersję aplikacji lub nie ma on żadnych wersji, to aplikacja nie znajdzie się w spisie
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

        private List<DirectoryInfo> GetDirectoryInfo(string path)
        {
            var info = new DirectoryInfo(path);
            return info.GetDirectories().ToList();
        }
    }
}
