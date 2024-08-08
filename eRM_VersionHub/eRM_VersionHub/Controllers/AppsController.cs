using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppsController(IAppDataScanner appDataScanner, ILogger<AppsController> logger)
        : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetStructure()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                logger.LogWarning(
                    AppLogEvents.Controller,
                    "GetStructure invoked but username not found"
                );
                return NotFound(ApiResponse<bool>.ErrorResponse(["Username not found"]));
            }

            logger.LogDebug(
                AppLogEvents.Controller,
                "GetStructure invoked with parameter: {username}",
                username
            );
            var response = await appDataScanner.GetAppsStructure(username);

            return Ok(ApiResponse<List<AppStructureDto>>.SuccessResponse(response).Serialize());
        }

        /*
         * usless for this moment
                [HttpGet("AppsNames")]
                [Authorize(Roles = "admin")]
                public IActionResult GetAppsNames()
                {
                    logger.LogDebug(AppLogEvents.Controller, "GetAppsNames invoked");

                    var response = appDataScanner.GetAppsNames();
                    logger.LogDebug(AppLogEvents.Controller, "GetAppsNames returned: {response}", response);

                    logger.LogInformation(
                        AppLogEvents.Controller,
                        "GetAppsNames returned data: {Data}",
                        response
                    );
                    return Ok(response.Serialize());
                }*/
    }
}
