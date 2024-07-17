using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublicationController(IOptions<AppSettings> appSettings, IPublicationService publicationService) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IPublicationService _publicationService = publicationService;


        [HttpPost]
        public void Post(List<VersionDto> versionDtos)
        {
            versionDtos.ForEach(version => _publicationService.Publish(_settings, version));

        }

        [HttpDelete]
        public void Delete(List<VersionDto> versionDtos)
        {
            versionDtos.ForEach(version => _publicationService.Unpublish(_settings, version));
        }
    }
}
