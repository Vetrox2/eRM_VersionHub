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
            string internalZipPath = Path.Combine(settings.InternalPackagesPath, $"{version.Name}.zip");
            
            using (FileStream internalZipStream = File.Open(internalZipPath, FileMode.Create, FileAccess.ReadWrite))
            using (ZipArchive internalZip = new(internalZipStream, ZipArchiveMode.Create, false))
            {
                version.Modules.ForEach(module =>
                {
                    string modulePath = Path.Combine(settings.InternalPackagesPath, module.Name, version.ID);
                    DirectoryInfo directoryInfo = new(modulePath);
                    FileInfo[] fileList = directoryInfo.GetFiles("*");
                    foreach (FileInfo file in fileList)
                    {
                        string sourcePath = Path.Combine(modulePath, file.Name);
                        string targetPath = Path.Combine(module.Name, version.ID, file.Name);
                        internalZip.CreateEntryFromFile(sourcePath, targetPath);
                    }
                });

                internalZipStream.Flush();  // Ensure all data is written to the file before calculating the hash
                string internalZipStringHash = StreamHashString(internalZipStream);
                internalZipStream.Close();  // Close the stream before copying the content
                CopyContent(internalZipPath, settings.ExternalPackagesPath, internalZipStringHash);
            }
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
            stream.Seek(0, SeekOrigin.Begin);  // Ensure we are reading the stream from the beginning
            byte[] byteHash = MD5.HashData(stream);
            return BitConverter.ToString(byteHash).Replace("-", "");
        }

        private static void CopyContent(string internalZipPath, string externalPackagesPath, string internalZipStringHash, int attempt = 0)
        {
            string fileName = Path.GetFileName(internalZipPath);
            string externalZipPath = Path.Combine(externalPackagesPath, fileName);

            File.Copy(internalZipPath, externalZipPath, true);

            using (FileStream externalZipStream = File.Open(externalZipPath, FileMode.Open, FileAccess.Read))
            {
                string externalZipStringHash = StreamHashString(externalZipStream);
                if (internalZipStringHash == externalZipStringHash)
                {
                    using (ZipArchive externalZip = new(externalZipStream, ZipArchiveMode.Read, false))
                    {
                        externalZip.ExtractToDirectory(externalPackagesPath);
                    }
                    
                    File.Delete(externalZipPath);
                    File.Delete(internalZipPath);
                    return;
                }
            }

            File.Delete(externalZipPath);
            if (attempt < _maxNumberOfAttempts)
            {
                CopyContent(internalZipPath, externalPackagesPath, internalZipStringHash, attempt + 1);
            }
            else
            {
                throw new Exception("Publishing failed");
            }
        }
    }
}