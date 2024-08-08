using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionService _permissionService;

        public PermissionController(
            IPermissionService permissionService,
            ILogger<PermissionController> logger
        )
        {
            _logger = logger;
            _permissionService = permissionService;
        }

        [HttpGet("{Username}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPermission(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked GetPermission with parameter: {Username}",
                Username
            );
            var result = await _permissionService.GetAllPermissionList(Username);
            _logger.LogInformation(
                AppLogEvents.Controller,
                "GetPermission returned: {Data}",
                result
            );
            return Ok(ApiResponse<AppPermissionDto>.SuccessResponse(result).Serialize());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPermission([FromBody] Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked AddPermission with data: {permission}",
                permission
            );
            var result = await _permissionService.CreatePermission(permission);
            _logger.LogInformation(
                AppLogEvents.Controller,
                "AddPermission returned: {Data}",
                result
            );
            return Ok(ApiResponse<Permission>.SuccessResponse(result).Serialize());
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePermission([FromBody] Permission permission)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked DeletePermission with data: {permission}",
                permission
            );
            var result = await _permissionService.DeletePermission(permission);
            _logger.LogInformation(
                AppLogEvents.Controller,
                "DeletePermission returned: {Data}",
                result
            );
            return Ok(ApiResponse<Permission>.SuccessResponse(result).Serialize());
        }
    }
}
