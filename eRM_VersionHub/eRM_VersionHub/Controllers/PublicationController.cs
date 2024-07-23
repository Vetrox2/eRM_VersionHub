using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController(IOptions<AppSettings> appSettings, IPublicationService publicationService, ILogger<PublicationController> logger) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IPublicationService _publicationService = publicationService;
        private readonly ILogger<PublicationController> _logger = logger; 

        [HttpPost]
        public IActionResult PublishVersions(List<VersionDto> versionDtos)
        {
            _logger.LogDebug(AppLogEvents.Controller, "PublishVersions invoked with data: {versionDtos}", versionDtos);
            if (versionDtos == null || versionDtos.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Data list for PublishVersions is empty");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Empty collection of versions to publish"]).Serialize());
            }

            List<string> result = [];
            foreach (VersionDto version in versionDtos)
            {
                _logger.LogDebug(AppLogEvents.Controller, "Publishing version: {version}", version);
                List<string> errors = _publicationService.Publish(_settings, version).Errors;

                if (errors.Count > 0)
                {
                    _logger.LogWarning(AppLogEvents.Controller, "Publish returned: {Errors}", errors);
                    result.AddRange(errors);
                }
            }

            _logger.LogInformation(AppLogEvents.Controller, "PublishVersions returned: {result}", result);
            return Ok(ApiResponse<bool>.ErrorResponse(result).Serialize());
        }

        [HttpDelete]
        public IActionResult UnpublishVersions(List<VersionDto> versionDtos)
        {
            _logger.LogDebug(AppLogEvents.Controller, "UnpublishVersions invoked with data: {versionDtos}", versionDtos);
            if (versionDtos == null || versionDtos.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Data list for UnpublishVersions is empty");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Empty collection of versions to unpublish"]).Serialize());
            }

            List<string> result = [];
            foreach (var version in versionDtos)
            {
                _logger.LogDebug(AppLogEvents.Controller, "Unpublishing version: {version}", version);
                List<string> modules = _publicationService.Unpublish(_settings, version).Errors;

                if (modules.Count > 0)
                {
                    _logger.LogWarning(AppLogEvents.Controller, "Unpublish returned: {modules}", modules);
                    result.AddRange(modules);
                }
            }

            _logger.LogInformation(AppLogEvents.Controller, "UnpublishVersions returned: {result}", result);
            return Ok(ApiResponse<bool>.ErrorResponse(result).Serialize());
        }
    }
}