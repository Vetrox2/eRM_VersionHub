using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController(IOptions<AppSettings> appSettings, IPublicationService publicationService,
        ILogger<PublicationController> logger, IPermissionService permissionService) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IPublicationService _publicationService = publicationService;
        private readonly IPermissionService _permissionService = permissionService;
        private readonly ILogger<PublicationController> _logger = logger;

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> PublishVersions(VersionDto versionDto)
        {
            _logger.LogDebug(AppLogEvents.Controller, "PublishVersions invoked with data: {versionDtos}", versionDto);

            if (versionDto == null || versionDto.Modules.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Data for PublishVersions is empty");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Empty version to publish"]).Serialize());
            }

            if (!await _permissionService.ValidatePermissions(versionDto, _settings, User?.Identity?.Name))
            {
                _logger.LogWarning(AppLogEvents.Controller, "User is not permitted to operate on these modules");
                return NotFound(ApiResponse<bool>.ErrorResponse(["User is not permitted to operate on these modules"]).Serialize());
            }

            List<string> result = [];

            _logger.LogDebug(AppLogEvents.Controller, "Publishing version: {version}", versionDto);
            List<string> errors = _publicationService.Publish(_settings, versionDto).Errors;

            if (errors.Count > 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Publish returned: {Errors}", errors);
                result.AddRange(errors);
            }

            _logger.LogInformation(AppLogEvents.Controller, "PublishVersions returned: {result}", result);
            return Ok(ApiResponse<bool>.ErrorResponse(result).Serialize());
        }

        [HttpDelete]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UnpublishVersions(VersionDto versionDto)
        {
            _logger.LogDebug(AppLogEvents.Controller, "UnpublishVersions invoked with data: {versionDtos}", versionDto);

            if (versionDto == null || versionDto.Modules.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Data for UnpublishVersions is empty");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Empty version to unpublish"]).Serialize());
            }

            if (!await _permissionService.ValidatePermissions(versionDto, _settings, User?.Identity?.Name))
            {
                _logger.LogWarning(AppLogEvents.Controller, "User is not permitted to operate on these modules");
                return NotFound(ApiResponse<bool>.ErrorResponse(["User is not permitted to operate on these modules"]).Serialize());
            }

            List<string> result = [];

            _logger.LogDebug(AppLogEvents.Controller, "Unpublishing version: {version}", versionDto);
            List<string> errors = _publicationService.Unpublish(_settings, versionDto).Errors;

            if (errors.Count > 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "Unpublish returned: {errors}", errors);
                result.AddRange(errors);
            }

            _logger.LogInformation(AppLogEvents.Controller, "UnpublishVersions returned: {result}", result);
            return Ok(ApiResponse<bool>.ErrorResponse(result).Serialize());
        }
    }
}