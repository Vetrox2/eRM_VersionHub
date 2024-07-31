using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger) : ControllerBase
    {
        [HttpGet("{Username}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPermission(string Username)
        {
            logger.LogDebug(AppLogEvents.Controller, "Invoked GetPermission with paramter: {Username}", Username);

            var result = await permissionService.GetPermissionList(Username);
            logger.LogDebug(AppLogEvents.Controller, "GetPermissionList result: {result}", result);

            if (result.Success)
            {
                logger.LogInformation(AppLogEvents.Controller, "GetPermission returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            logger.LogWarning(AppLogEvents.Controller, "GetFavorites returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPermssion([FromBody] Permission permission)
        {
            logger.LogDebug(AppLogEvents.Controller, "Invoked AddPermssion with data: {permission}", permission);

            ApiResponse<Permission?> result = await permissionService.CreatePermission(permission);
            logger.LogDebug(AppLogEvents.Controller, "CreatePermission result: {result}", result);

            if (result.Success)
            {
                logger.LogInformation(AppLogEvents.Controller, "AddPermssion returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            logger.LogWarning(AppLogEvents.Controller, "AddPermssion returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePermission([FromBody] Permission permission)
        {
            logger.LogDebug(AppLogEvents.Controller, "Invoked DeletePermission with data: {permission}", permission);

            ApiResponse<Permission?> result = await permissionService.DeletePermission(permission);
            logger.LogDebug(AppLogEvents.Controller, "DeletePermission result: {result}", result);

            if (result.Success)
            {
                logger.LogInformation(AppLogEvents.Controller, "DeletePermission returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            logger.LogWarning(AppLogEvents.Controller, "DeletePermission returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }
    }
}