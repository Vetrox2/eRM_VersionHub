using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IPublicationService
    {
        public void Publish(MyAppSettings settings, VersionDto version);
        public void Unpublish(MyAppSettings settings, VersionDto version);
    }
}