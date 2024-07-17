using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using System.IO.Compression;
using System.Security.Cryptography;

namespace eRM_VersionHub.Services
{
    public class PublicationService : IPublicationService
    {
        private readonly static int _maxNumberOfAttempts = 5;
        
        public ApiResponse<bool> Publish(MyAppSettings settings, VersionDto version)
        {
            foreach (ModuleDto module in version.Modules)
            {
                string versionPath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                if (!Directory.Exists(versionPath))
                {
                    return ApiResponse<bool>.ErrorResponse([$"Module \"{module.Name}\" or version \"{version.ID}\" does not exist"]);
                }
            }

            foreach (ModuleDto module in version.Modules)
            {
                string versionPath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                string internalZipPath = Path.Combine(settings.InternalPackagesPath, module.Name, $"{version.Name}.zip");
                using (FileStream internalZipStream = File.Open(internalZipPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    ZipFile.CreateFromDirectory(versionPath, internalZipStream);
                    internalZipStream.Flush();
                    string internalZipStringHash = StreamHashString(internalZipStream);
                    internalZipStream.Close();
                    bool response = CopyContent(internalZipPath, settings.ExternalPackagesPath, module.Name, version.Name, internalZipStringHash);
                    if (!response)
                    {
                        Unpublish(settings, version);
                        return ApiResponse<bool>.ErrorResponse([$"System could not publish module \"{module.Name} v{version.ID}\". Rollbacking publication of this version."]);
                    }
                }
            }
            return ApiResponse<bool>.ErrorResponse([]);
        }

        public ApiResponse<bool> Unpublish(MyAppSettings settings, VersionDto version)
        {
            IEnumerable<string> skippedModules = version.Modules.Select(module => 
            {
                string targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, version.ID);
                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
                return $"Module \"{module.Name}\" version \"{version.ID}\"";
            });
            return ApiResponse<bool>.ErrorResponse(skippedModules.ToList());
        }

        private static string StreamHashString(FileStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteHash = MD5.HashData(stream);
            return BitConverter.ToString(byteHash).Replace("-", "");
        }

        private static bool CopyContent(string internalZipPath, string externalPackagesPath, string moduleName, string version, string internalZipStringHash, int attempt = 0)
        {
            string fileName = Path.GetFileName(externalPackagesPath);
            string versionFolder = Path.Combine(externalPackagesPath, moduleName, version);
            Directory.CreateDirectory(versionFolder);
            string externalZipPath = Path.Combine(versionFolder, fileName);

            File.Copy(internalZipPath, externalZipPath, true);

            using (FileStream externalZipStream = File.Open(externalZipPath, FileMode.Open, FileAccess.Read))
            {
                string externalZipStringHash = StreamHashString(externalZipStream);
                if (internalZipStringHash == externalZipStringHash)
                {
                    using (ZipArchive externalZip = new(externalZipStream, ZipArchiveMode.Read, false))
                    {
                        externalZip.ExtractToDirectory(versionFolder);
                    }
                    
                    File.Delete(externalZipPath);
                    File.Delete(internalZipPath);
                    return true;
                }
            }

            File.Delete(externalZipPath);
            if (attempt < _maxNumberOfAttempts)
            {
                return CopyContent(internalZipPath, externalPackagesPath, moduleName, version, internalZipStringHash, attempt + 1);
            }
            else
            {
                return false;
            }
        }
    }
}