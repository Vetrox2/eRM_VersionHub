using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using System;

namespace eRM_VersionHub.Services
{
    public class TagService : ITagService
    {

        public ApiResponse<string> SetTag(MyAppSettings _settings, string appID, string versionID, string newTag = "")
        {
            var appModel = AppDataScanner.GetAppJsonModel(Path.Combine(_settings.AppsPath, appID, _settings.ApplicationConfigFile));
            if (appModel == null || appModel.Modules.Count == 0)
                return ApiResponse<string>.ErrorResponse(["App not found"]);

            int internalModulesModified = 0, publishedModulesModified = 0;
            var newVersionID = SwapVersionTag(versionID, newTag);

            if(newVersionID == versionID)
                return ApiResponse<string>.ErrorResponse(["New tag is the same as the old one"]);

            appModel.Modules.ForEach(module =>
            {
                internalModulesModified += ChangeTagOnPath(_settings.InternalPackagesPath, module.ModuleId, versionID, newVersionID) ? 1 : 0;
                publishedModulesModified += ChangeTagOnPath(_settings.ExternalPackagesPath, module.ModuleId, versionID, newVersionID) ? 1 : 0;
            });

            if (internalModulesModified == 0)
                return ApiResponse<string>.ErrorResponse(["Version for none module was modified"]);

            return ApiResponse<string>.SuccessResponse(
                $"Internal modules version modified: {internalModulesModified}\nPublished modules version modified: {publishedModulesModified}");
        }

        public static (string Name, string Tag) SplitVersionID(string versionID)
        {
            string Name, Tag;
            var index = versionID.IndexOf('-');

            if (index == -1)
            {
                Name = versionID;
                Tag = string.Empty;
            }
            else
            {
                Name = versionID.Substring(0, index);
                Tag = versionID[(index + 1)..];
            }
            return (Name, Tag);
        }

        public static string SwapVersionTag(string versionID, string newTag)
        {
            var (newVersionID, _) = SplitVersionID(versionID);
            if (!string.IsNullOrEmpty(newTag))
                newVersionID += $"-{newTag}";

            return newVersionID;
        }

        private bool ChangeTagOnPath(string packagesPath, string moduleId, string versionID, string newVersionID)
        {
            var oldPath = Path.Combine(packagesPath, moduleId, versionID);
            var newPath = Path.Combine(packagesPath, moduleId, newVersionID);

            if (Directory.Exists(oldPath))
            {
                Directory.Move(oldPath, newPath);
                return true;
            }

            return false;
        }
    }
}
