using System.Collections.Concurrent;

namespace eRM_VersionHub.Services
{
    public static class FolderLockManager
    {
        private static readonly ConcurrentDictionary<string, object> FolderLocks = new ConcurrentDictionary<string, object>();

        public static object GetOrAdd(string path) => FolderLocks.GetOrAdd(path, new object());

        public static void TryRemove(string path) => FolderLocks.TryRemove(path, out _);
    }
}
