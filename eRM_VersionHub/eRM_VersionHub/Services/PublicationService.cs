using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services
{
    public class PublicationService(ILogger<PublicationService> logger) : IPublicationService
    {
        private readonly ILogger<PublicationService> _logger = logger;
        public ApiResponse<bool> Publish(MyAppSettings settings, VersionDto version)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked Publish with data: {settings}\n{version}", settings, version);
            if (version == null || version.Modules == null)
            {
                _logger.LogWarning(AppLogEvents.Service, "Version list for Publish is empty");
                return ApiResponse<bool>.ErrorResponse(["Empty collection of modules to publish"]);
            }

            List<string> errors = [];
            foreach (var module in version.Modules)
            {
                string sourcePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                _logger.LogDebug(AppLogEvents.Service, "Checking version in: {sourcePath}", sourcePath);

                if (!Directory.Exists(sourcePath))
                {
                    _logger.LogWarning(AppLogEvents.Service, "This version doesn't exist: {Name}, {ID}", module.Name, version.ID);
                    errors.Add($"Module \"{module.Name}\" version \"{version.ID}\" does not exist");
                }
            }

            if (errors.Count > 0)
            {
                _logger.LogWarning(AppLogEvents.Service, "Publish returned: {errors}", errors);
                return ApiResponse<bool>.ErrorResponse(errors);
            }

            foreach (var module in version.Modules)
            {
                var publishedModule = AppDataScanner.GetModuleModels(settings.ExternalPackagesPath, [module.Name]);
                var publishedVersionID = publishedModule[0].Versions.FirstOrDefault(publishedVersion => TagService.CompareVersions(publishedVersion, version.ID));
                
                _logger.LogDebug(AppLogEvents.Service, "Publishing module: {publishedModule}, {publishedVersionID}", publishedModule, publishedVersionID);
                if (!string.IsNullOrEmpty(publishedVersionID))
                {
                    var success = TagService.ChangeTagOnPath(settings.ExternalPackagesPath, module.Name, publishedVersionID, TagService.SwapVersionTag(version.ID, version.PublishedTag));
                    _logger.LogDebug(AppLogEvents.Service, "ChangeTagOnPath returned: {success}", success);

                    if (success)
                    {
                        _logger.LogInformation(AppLogEvents.Service, "Successfully changing tag for: {publishedModule}, {publishedVersionID}", publishedModule, publishedVersionID);
                        continue;
                    }

                    _logger.LogWarning(AppLogEvents.Service, "Unpublishing due to failure of changing tag in module: {publishedModule}, {publishedVersionID}", publishedModule, publishedVersionID);
                    Unpublish(settings, version);

                    return ApiResponse<bool>.ErrorResponse([$"System could not change published tag on module \"{module.Name}\" version \"{version.ID}\". Rollbacking publication of this version."]);
                }

                var sourcePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, TagService.SwapVersionTag(version.ID, version.PublishedTag));
                
                PrepareTargetPath(settings.ExternalPackagesPath, module.Name, targetPath);
                var response = CopyContent(sourcePath, targetPath);

                _logger.LogDebug(AppLogEvents.Service, "CopyContent returned: {response}", response);
                if (!response.Success)
                {
                    _logger.LogWarning(AppLogEvents.Service, "Unpublishing due to failure of copying {sourcePath} to {targetPath}", sourcePath, targetPath);
                    Unpublish(settings, version);

                    response.Errors.Add($"System could not publish module \"{module.Name}\" version \"{version.ID}\". Rollbacking publication of this version.");
                    _logger.LogWarning(AppLogEvents.Service, "Publish returned: {Errors}", response.Errors);

                    return ApiResponse<bool>.ErrorResponse(response.Errors);
                }
            }

            _logger.LogInformation(AppLogEvents.Service, "Successfully publishing: {version}", version);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public ApiResponse<bool> Unpublish(MyAppSettings settings, VersionDto version)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked Unpublish with data: {settings}\n{version}", settings, version);

            if (version == null || version.Modules == null)
            {
                _logger.LogWarning(AppLogEvents.Service, "Version list for Unpublish is empty");
                return ApiResponse<bool>.ErrorResponse(["Empty collection of modules to unpublish"]);
            }

            foreach (var module in version.Modules)
            {
                var publishedModule = AppDataScanner.GetModuleModels(settings.ExternalPackagesPath, [module.Name]);
                var publishedVersionID = publishedModule[0].Versions.FirstOrDefault(publishedVersion => TagService.CompareVersions(publishedVersion, version.ID));
                
                _logger.LogDebug(AppLogEvents.Service, "Unpublishing module: {publishedModule}, {publishedVersionID}", publishedModule, publishedVersionID);
                if (string.IsNullOrEmpty(publishedVersionID))
                {
                    _logger.LogWarning(AppLogEvents.Service, "This module doesn't exist: {publishedVersionID}", publishedVersionID);
                    continue;
                }

                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, publishedVersionID);
                if (Directory.Exists(targetPath))
                {
                    _logger.LogDebug(AppLogEvents.Service, "Deleting folder: {targetPath}", targetPath);
                    Directory.Delete(targetPath, true);
                }
            }

            _logger.LogInformation(AppLogEvents.Service, "Unpublishing version: {version}", version);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        private void PrepareTargetPath(string ExternalPackagesPath, string module, string targetPath)
        {
            _logger.LogDebug(AppLogEvents.Service, "Preparing target path with data: {ExternalPackagesPath}, {module}, {targetPath}",
                ExternalPackagesPath, module, targetPath);
            var modulePath = Path.Combine(ExternalPackagesPath, module);

            if (!Directory.Exists(modulePath))
            {
                _logger.LogWarning(AppLogEvents.Service, "Creating directory that doesn't exist: {modulePath}", modulePath);
                Directory.CreateDirectory(modulePath);
            }

            if (Directory.Exists(targetPath))
            {
                _logger.LogWarning(AppLogEvents.Service, "Deleting directory that exists: {modulePath}", targetPath);
                Directory.Delete(targetPath, true);
            }

            _logger.LogInformation(AppLogEvents.Service, "Creating directory: {targetPath}", targetPath);
            Directory.CreateDirectory(targetPath);
        }

        private ApiResponse<bool> CopyContent(string sourcePath, string targetPath)
        {
            _logger.LogDebug(AppLogEvents.Service, "Copying {sourcePath} to {targetPath}", sourcePath, targetPath);

            try
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    var newPath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, dirPath));
                    _logger.LogDebug(AppLogEvents.Service, "Copying {dirPath} to {newPath}", dirPath, newPath);

                    if (!Directory.Exists(newPath))
                    {
                        _logger.LogDebug(AppLogEvents.Service, "Creating directory {newPath}", newPath);
                        Directory.CreateDirectory(newPath);
                    }
                }

                foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    var newPath = Path.Combine(targetPath, Path.GetRelativePath(sourcePath, filePath));
                    _logger.LogDebug(AppLogEvents.Service, "Copying {filePath} to {newPath}", filePath, newPath);
                    File.Copy(filePath, newPath, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Service, "When copying content, this exception was thrown: {Message}\n{StackTrace}", ex.Message, ex.StackTrace);
                return ApiResponse<bool>.ErrorResponse([]);
            }

            _logger.LogInformation(AppLogEvents.Service, "This version has been copied: {sourcePath}", sourcePath);
            return ApiResponse<bool>.SuccessResponse(true);

            //add checksum
        }
    }
}