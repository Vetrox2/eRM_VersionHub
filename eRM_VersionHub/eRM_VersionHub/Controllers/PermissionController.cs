using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger) : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger = logger;
        private readonly IPermissionService _permissionService = permissionService;

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetPermission(string Username)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetPermission with paramter: {Username}", Username);
            ApiResponse<List<Permission>> result = await _permissionService.GetPermissionList(Username);
            _logger.LogDebug(AppLogEvents.Controller, "GetPermissionList result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "GetPermission returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "GetFavorites returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost]
        public async Task<IActionResult> AddPermssion([FromBody] Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked AddPermssion with data: {permission}", permission);
            ApiResponse<Permission?> result = await _permissionService.CreatePermission(permission);
            _logger.LogDebug(AppLogEvents.Controller, "CreatePermission result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "AddPermssion returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "AddPermssion returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] Permission permission)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked DeletePermission with data: {permission}", permission);
            ApiResponse<Permission?> result = await _permissionService.DeletePermission(permission);
            _logger.LogDebug(AppLogEvents.Controller, "DeletePermission result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "DeletePermission returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "DeletePermission returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }
    }
}