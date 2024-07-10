using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            ApiResponse<List<Permission>> result = await _permissionService.GetPermissionList(Username);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }

        [HttpPost]
        public async Task<IActionResult> AddPermssion([FromBody] Permission permission)
        {
            ApiResponse<Permission?> result = await _permissionService.CreatePermission(permission);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] Permission permission)
        {
            ApiResponse<Permission?> result = await _permissionService.DeletePermission(permission);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }
    }
}