using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using Microsoft.AspNetCore.Mvc;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController(IPermissionService permissionService) : ControllerBase
    {
        private readonly IPermissionService _permissionService = permissionService;

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetPermission(string Username)
        {
            Result<List<Permission>> result = await _permissionService.GetPermissionList(Username);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddPermssion([FromBody] Permission permission)
        {
            Result<Permission?> result = await _permissionService.CreatePermission(permission);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] Permission permission)
        {
            Result<Permission?> result = await _permissionService.DeletePermission(permission);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }
    }
}