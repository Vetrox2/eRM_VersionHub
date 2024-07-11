using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface ITagService
    {
        public ApiResponse<string> SetTag(MyAppSettings _settings, string appID, string versionID, string newTag = "");
    }
}
