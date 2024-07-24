using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace eRM_VersionHub.Services
{
    public class PublicationService(ILogger<PublicationService> logger) : IPublicationService
    {
        private readonly ILogger<PublicationService> _logger = logger;

        public ApiResponse<bool> Publish(MyAppSettings settings, VersionDto version)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked Publish with data: {settings}\n{version}", settings, version);

            if (ValidateVersionDto(version))
                return ApiResponse<bool>.ErrorResponse(["Empty collection of modules to publish"]);

            List<string> errors = [];

            if (ValidateAllModulesExistence(version, settings, errors))
            {
                _logger.LogWarning(AppLogEvents.Service, "Publish returned: {errors}", errors);
                return ApiResponse<bool>.ErrorResponse(errors);
            }

            foreach (var module in version.Modules)
            {
                var (doContinue, returnError) = ChangeTagIfModuleIsPublished(settings, version, module);
                if(doContinue)
                    continue;
                if(returnError)
                    return ApiResponse<bool>.ErrorResponse([$"System could not change published tag on module \"{module.Name}\" version \"{version.ID}\"" +
                        $". Rollbacking publication of this version."]);

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

            _logger.LogInformation(AppLogEvents.Service, "Successful publication of: {version}", version);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public ApiResponse<bool> Unpublish(MyAppSettings settings, VersionDto version)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked Unpublish with data: {settings}\n{version}", settings, version);

            if (ValidateVersionDto(version))
                return ApiResponse<bool>.ErrorResponse(["Empty collection of modules to unpublish"]);

            List<string> errors = [];

            foreach (var module in version.Modules)
            {
                var publishedVersionID = GetPublishedVersionID(settings, module, version);
                _logger.LogDebug(AppLogEvents.Service, "Unpublishing module: {module.Name}, {publishedVersionID}", module.Name, publishedVersionID);

                if (string.IsNullOrEmpty(publishedVersionID))
                {
                    _logger.LogWarning(AppLogEvents.Service, "This version is not published: {publishedVersionID}", publishedVersionID);
                    continue;
                }

                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, publishedVersionID);

                try 
                {
                    _logger.LogDebug(AppLogEvents.Service, "Deleting folder: {targetPath}", targetPath);
                    Directory.Delete(targetPath, true);
                }
                catch
                {
                    _logger.LogDebug(AppLogEvents.Service, "An error occurred while deleting the version: {module.Name} {publishedVersionID}", 
                        module.Name, publishedVersionID);
                    errors.Add($"An error occurred while deleting the version: {module.Name} {publishedVersionID}");
                }
            }

            _logger.LogInformation(AppLogEvents.Service, "Unpublishing version: {version}", version);
            return ApiResponse<bool>.ErrorResponse(errors);
        }

        private bool ValidateVersionDto(VersionDto version)
        {
            if (version == null || version.Modules == null)
            {
                _logger.LogWarning(AppLogEvents.Service, "Version list is empty");
                return true;
            }

            return false;
        }

        private bool ValidateAllModulesExistence(VersionDto version, MyAppSettings settings, List<string> errors)
        {
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

            return errors.Count > 0;
        }

        private string? GetPublishedVersionID(MyAppSettings settings, ModuleDto module, VersionDto version)
        {
            var publishedModule = AppDataScanner.GetModuleModels(settings.ExternalPackagesPath, [module.Name]);
            return publishedModule[0].Versions.FirstOrDefault(publishedVersion => TagService.CompareVersions(publishedVersion, version.ID));
        }

        private (bool doContinue, bool returnError) ChangeTagIfModuleIsPublished(MyAppSettings settings, VersionDto version, ModuleDto module)
        {
            var publishedVersionID = GetPublishedVersionID(settings, module, version);

            if (!string.IsNullOrEmpty(publishedVersionID))
            {
                _logger.LogDebug(AppLogEvents.Service, "Module is already published: {module.Name}, {publishedVersionID}.\nTrying to change its tag",
                    module.Name, publishedVersionID);

                var success = TagService.ChangeTagOnPath(settings.ExternalPackagesPath, module.Name, publishedVersionID, TagService.SwapVersionTag(version.ID, version.PublishedTag));

                if (success)
                {
                    _logger.LogInformation(AppLogEvents.Service, "Successfully changing tag for: {module.Name}, {publishedVersionID}", module.Name, publishedVersionID);
                    return (true, false);
                }

                _logger.LogWarning(AppLogEvents.Service, "Unpublishing due to failure of changing tag in module: {module.Name}, {publishedVersionID}", module.Name, publishedVersionID);
                Unpublish(settings, version);
                return (false, true);
            }

            return (false, false);
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