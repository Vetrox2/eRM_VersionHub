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
    public class PublicationController(IOptions<AppSettings> appSettings, IPublicationService publicationService, ILogger<PublicationController> logger, IPermissionService permissionService) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;

        [HttpPost]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> PublishVersions(VersionDto versionDto)
        {
            return await HandleVersionOperation(versionDto, publicationService.Publish, "PublishVersions");
        }

        [HttpDelete]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> UnpublishVersions(VersionDto versionDto)
        {
            return await HandleVersionOperation(versionDto, publicationService.Unpublish, "UnpublishVersions");
        }

        private async Task<IActionResult> HandleVersionOperation(VersionDto versionDto, Func<MyAppSettings, VersionDto, ApiResponse<bool>> operation, string operationName)
        {
            logger.LogDebug(AppLogEvents.Controller, "PublishVersions invoked with data: {versionDtos}", versionDto);

            if (versionDto == null || versionDto.Modules.Count == 0)
            {
                logger.LogWarning(AppLogEvents.Controller, "Data for PublishVersions is empty");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Request does not contain any modules to publish"]).Serialize());
            }

            if (!await permissionService.ValidatePermissions(versionDto, _settings, User?.Identity?.Name))
            {
                logger.LogWarning(AppLogEvents.Controller, "User is not permitted to operate on these modules");
                return Forbid(ApiResponse<bool>.ErrorResponse(["User is not permitted to operate on these modules"]).Serialize());
            }

            logger.LogDebug(AppLogEvents.Controller, "{operationName} version: {version}", operationName,versionDto);
            var errors = operation(_settings, versionDto).Errors;

            if (errors.Count > 0)
            {
                logger.LogWarning(AppLogEvents.Controller, "{operationName} returned: {Errors}", operationName, errors);
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(errors).Serialize());
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true).Serialize());
        }
    }
}