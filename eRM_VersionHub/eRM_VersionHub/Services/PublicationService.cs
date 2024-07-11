using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using static Dapper.SqlMapper;

namespace eRM_VersionHub.Services
{
    public class PublicationService : IPublicationService
    {
        public void Publish(MyAppSettings settings, VersionDto version)
        {
            version.Modules.ForEach(module =>
                {
                    var sourcePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                    var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, version.ID);

                    PrepareTargetPath(settings.ExternalPackagesPath, module.Name, targetPath);
                    CopyContent(sourcePath, targetPath);
                });
        }

        public void Unpublish(MyAppSettings settings, VersionDto version)
        {
            version.Modules.ForEach(module =>
            {
                var targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, version.ID);
                if (Directory.Exists(targetPath))
                    Directory.Delete(targetPath, true);
            });
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

        private void CopyContent(string sourcePath, string targetPath)
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

            //add checksum
        }
    }
}
