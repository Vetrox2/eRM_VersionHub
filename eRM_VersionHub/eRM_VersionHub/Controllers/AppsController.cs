using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppsController(IOptions<AppSettings> appSettings, IAppDataScanner appDataScanner)
        : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IAppDataScanner _appDataScanner = appDataScanner;

        [HttpGet("{UserName}")]
        public async Task<IActionResult> GetStructure(string UserName)
        {
            var structure = await _appDataScanner.GetAppsStructure(_settings, UserName);
            if (structure == null || structure.Count == 0)
                return NotFound(ApiResponse<string>.ErrorResponse(["Some error"]).Serialize());

            return Ok(
                ApiResponse<List<Dtos.AppStructureDto>>.SuccessResponse(structure).Serialize()
            );
        }
    }
}
