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
    public class AppsController(IAppDataScanner appDataScanner, ILogger<AppsController> logger) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStructure()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(AppLogEvents.Controller, "GetStructure invoked but username not found");
                return NotFound(ApiResponse<bool>.ErrorResponse(["Username not found"]));
            }

            logger.LogDebug(AppLogEvents.Controller, "GetStructure invoked with parameter: {username}", username);
            var response = await appDataScanner.GetAppsStructure(username);

            if (response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning(AppLogEvents.Controller, "GetStructure returned: {Errors}", response.Errors);
                return NotFound(response.Serialize());
            }

            logger.LogInformation(AppLogEvents.Controller, "GetStructure returned: {Data}", response.Data);
            return Ok(response.Serialize());
        }

        [HttpGet("AppsNames")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAppsNames()
        {
            logger.LogDebug(AppLogEvents.Controller, "GetAppsNames invoked");

            var response = appDataScanner.GetAppsNames();
            logger.LogDebug(AppLogEvents.Controller, "GetAppsNames returned: {response}", response);

            if (!response.Success || response.Data == null || response.Data.Count == 0)
            {
                logger.LogWarning(AppLogEvents.Controller, "GetAppsNames returned: {Errors}", response.Errors);
                return NotFound(response.Serialize());
            }

            logger.LogInformation(AppLogEvents.Controller, "GetAppsNames returned data: {Data}", response.Data);
            return Ok(response.Serialize());
        }
    }
}