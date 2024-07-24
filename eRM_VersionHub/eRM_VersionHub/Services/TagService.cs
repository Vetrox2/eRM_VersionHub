using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using System;
using System.ComponentModel;

namespace eRM_VersionHub.Services
{
    public static class TagService
    {

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

        public static string GetVersionWithoutTag(string versionID)
        {
            var index = versionID.IndexOf('-');
            return index == -1 ? versionID : versionID.Substring(0, index);
        }

        public static string GetTag(string versionID)
        {
            var index = versionID.IndexOf('-');
            return index == -1 ? "" : versionID.Substring(index + 1);
        }

        public static string SwapVersionTag(string versionID, string newTag)
        {
            var (newVersionID, _) = SplitVersionID(versionID);
            if (!string.IsNullOrEmpty(newTag))
                newVersionID += $"-{newTag}";

            return newVersionID;
        }

        /// <summary>
        /// Compares version numbers without their tags.
        /// </summary>
        public static bool CompareVersions(string versionID1, string versionID2)
            => GetVersionWithoutTag(versionID1) == GetVersionWithoutTag(versionID2);

        public static bool ChangeTagOnPath(string packagesPath, string moduleId, string versionID, string newVersionID)
        {

            if (!TagService.CompareVersions(versionID, newVersionID))
                return false;

            var oldPath = string.Empty;
            var newPath = Path.Combine(packagesPath, moduleId, newVersionID);
            var publishedModulePath = Path.Combine(packagesPath, moduleId);

            if (!Directory.Exists(publishedModulePath))
                return false;

            var info = new DirectoryInfo(publishedModulePath);
            var allPublishedVersions = info.GetDirectories().ToList();
            foreach (var publishedVersion in allPublishedVersions)
            {
                if (GetVersionWithoutTag(publishedVersion.Name) == (GetVersionWithoutTag(versionID)))
                {
                    oldPath = Path.Combine(packagesPath, moduleId, publishedVersion.Name);
                    break;
                }
            }
            if (string.IsNullOrEmpty(oldPath))
                return false;

            if (oldPath == newPath)
                return true;

            if (Directory.Exists(oldPath))
            {
                var oldLock = FolderLockManager.GetOrAdd(oldPath);
                var newLock = FolderLockManager.GetOrAdd(newPath);

                lock (oldLock)
                {
                    lock (newLock)
                    {

                        try
                        {
                            Directory.Move(oldPath, newPath);
                        }
                        catch
                        {
                            return false;
                        }
                        finally
                        {
                            FolderLockManager.TryRemove(oldPath);
                            FolderLockManager.TryRemove(newPath);
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
