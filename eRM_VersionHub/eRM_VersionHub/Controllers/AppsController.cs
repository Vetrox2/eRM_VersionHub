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
    public class AppsController(IOptions<AppSettings> appSettings, IAppDataScanner appDataScanner, ILogger<AppsController> logger) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IAppDataScanner _appDataScanner = appDataScanner;
        private readonly ILogger<AppsController> _logger = logger;

        [HttpGet]
        [Authorize(Roles ="user")]
        public async Task<IActionResult> GetStructure()
        {
            var UserName = User.Identity?.Name;
            if (string.IsNullOrEmpty(UserName))
            {
                _logger.LogWarning(AppLogEvents.Controller, "GetStructure invoked but userName not found");
                return NotFound(ApiResponse<bool>.ErrorResponse(["UserName not found"]));
            }

            _logger.LogDebug(AppLogEvents.Controller, "GetStructure invoked with paramter: {UserName}", UserName);
            var response = await _appDataScanner.GetAppsStructure(_settings, UserName);
            _logger.LogDebug(AppLogEvents.Controller, "GetAppsStructure returned: {UserName}", UserName);

            if (response.Data == null || response.Data.Count == 0)
            {
                _logger.LogWarning(AppLogEvents.Controller, "GetStructure returned: {Errors}", response.Errors);
                return NotFound(response.Serialize());
            }

            _logger.LogInformation(AppLogEvents.Controller, "GetStructure returned: {Data}", response.Data);
            return Ok(response.Serialize());
        }
    }
}