using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using System.IO.Compression;
using System.Security.Cryptography;

namespace eRM_VersionHub.Services
{
    public class PublicationService : IPublicationService
    {
        private static int _maxNumberOfAttempts = 5;
        
        public void Publish(MyAppSettings settings, VersionDto version)
        {
            version.Modules.ForEach(module =>
            {
                string versionPath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                string internalZipPath = Path.Combine(settings.InternalPackagesPath, module.Name, $"{version.Name}.zip");
                using (FileStream internalZipStream = File.Open(internalZipPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    ZipFile.CreateFromDirectory(versionPath, internalZipStream);
                    internalZipStream.Flush();
                    string internalZipStringHash = StreamHashString(internalZipStream);
                    internalZipStream.Close();
                    CopyContent(internalZipPath, settings.ExternalPackagesPath, module.Name, version.Name, internalZipStringHash);
                }
            });
        }

        public void Unpublish(MyAppSettings settings, VersionDto version)
        {
            version.Modules.ForEach(module =>
            {
                string targetPath = Path.Combine(settings.ExternalPackagesPath, module.Name, version.ID);
                if (Directory.Exists(targetPath))
                    Directory.Delete(targetPath, true);
            });
        }

        private static string StreamHashString(FileStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] byteHash = MD5.HashData(stream);
            return BitConverter.ToString(byteHash).Replace("-", "");
        }

        private static void CopyContent(string internalZipPath, string externalPackagesPath, string moduleName, string version, string internalZipStringHash, int attempt = 0)
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
                    return;
                }
            }

            File.Delete(externalZipPath);
            if (attempt < _maxNumberOfAttempts)
            {
                CopyContent(internalZipPath, externalPackagesPath, moduleName, version, internalZipStringHash, attempt + 1);
            }
            else
            {
                throw new Exception("Publishing failed");
            }
        }
    }
}