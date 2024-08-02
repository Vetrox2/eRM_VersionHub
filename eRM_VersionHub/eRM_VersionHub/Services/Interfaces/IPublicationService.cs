using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPublicationService
    {
        public Task<ApiResponse<bool>> Publish(MyAppSettings settings, VersionDto version);
        public Task<ApiResponse<bool>> Unpublish(MyAppSettings settings, VersionDto version);
    }
}