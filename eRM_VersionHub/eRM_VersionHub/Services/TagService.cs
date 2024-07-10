using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services
{
    public class TagService : ITagService
    {

        public ApiResponse<string> SetTag(MyAppSettings _settings, string appID, string versionID, string newTag = "")
        {
            var appModel = AppDataScanner.GetAppJsonModel(Path.Combine(_settings.AppsPath, appID, _settings.ApplicationConfigFile));
            if (appModel == null || appModel.Modules.Count == 0)
                return ApiResponse<string>.ErrorResponse(["App not found"]);

            appModel.Modules.ForEach(module =>
            {
                var (newName, _) = SplitVersionID(versionID);
                if (!string.IsNullOrEmpty(newTag))
                    newName += $"-{newTag}";

                var internalPath = Path.Combine(_settings.InternalPackagesPath, module.ModuleId, versionID);
                var externalPath = Path.Combine(_settings.ExternalPackagesPath, module.ModuleId, versionID);

                var newInternalPath = Path.Combine(_settings.InternalPackagesPath, module.ModuleId, newName);
                var newExternalPath = Path.Combine(_settings.ExternalPackagesPath, module.ModuleId, newName);

                if (Directory.Exists(internalPath))
                    Directory.Move(internalPath, newInternalPath);
                if (Directory.Exists(externalPath))
                    Directory.Move(externalPath, newExternalPath);
            });

            return ApiResponse<string>.SuccessResponse("Success");
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
    }
}
