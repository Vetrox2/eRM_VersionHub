using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services
{
    public class PublicationService : IPublicationService
    {
        public ApiResponse<bool> Publish(MyAppSettings settings, VersionDto version)
        {
            List<string> errors = [];
            foreach (var module in version.Modules)
            {
                string sourcePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                if (!Directory.Exists(sourcePath))
                    errors.Add($"Module \"{module.Name}\" version \"{version.ID}\" does not exist");
            }

            if (errors.Count > 0)
                return ApiResponse<bool>.ErrorResponse(errors);

            foreach (var module in version.Modules)
            {
                var sourcePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, TagService.SwapVersionTag(version.ID,version.Tag));//TODO

                PrepareTargetPath(settings.ExternalPackagesPath, module.Name, targetPath);
                var response = CopyContent(sourcePath, targetPath);

                if (!response.Success)
                {
                    Unpublish(settings, version);
                    response.Errors.Add($"System could not publish module \"{module.Name}\" version \"{version.ID}\". Rollbacking publication of this version.");
                    return ApiResponse<bool>.ErrorResponse(response.Errors);//Returning basic error from try catch, check if there is some sensitive data
                }
            }

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public ApiResponse<bool> Unpublish(MyAppSettings settings, VersionDto version)
        {
            List<string> errors = [];
            foreach (var module in version.Modules)
            {
                //TODO - correct when rollbacking, uncorrect when unpublishing --------------------here
                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, TagService.SwapVersionTag(version.ID, version.Tag));
                if (!Directory.Exists(targetPath))
                {
                    errors.Add($"{module.Name} could not be unpublished, because was not found");
                    continue;
                }

                Directory.Delete(targetPath, true);
            };

            return ApiResponse<bool>.ErrorResponse(errors);
        }

        private void PrepareTargetPath(string ExternalPackagesPath, string module, string targetPath)
        {
            var modulePath = Path.Combine(ExternalPackagesPath, module);
            if (!Directory.Exists(modulePath))
                Directory.CreateDirectory(modulePath);

            if (Directory.Exists(targetPath))
                Directory.Delete(targetPath, true);

            Directory.CreateDirectory(targetPath);
        }

        private ApiResponse<bool> CopyContent(string sourcePath, string targetPath)
        {
            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    var newPath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, dirPath));
                    if (!Directory.Exists(newPath))
                        Directory.CreateDirectory(newPath);
                }

                foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    var newPath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, filePath));
                    System.IO.File.Copy(filePath, newPath, true);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse([ex.Message]);
            }

            return ApiResponse<bool>.SuccessResponse(true);

            //add checksum
        }
    }
}
