using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
    {
        [HttpGet("UsersWithApps")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersWithApps()
        {
            logger.LogDebug(AppLogEvents.Controller, "Invoked GetUsersWithApps");

            var result = await userService.GetUsersWithApps();
            logger.LogDebug(AppLogEvents.Controller, "GetUsersWithApps result: {result}", result);

            if (!result.Success || result.Data == null)
            {
                logger.LogWarning(AppLogEvents.Controller, "GetUser returned error(s): {Errors}", result.Errors);
                return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
            }

            logger.LogInformation(AppLogEvents.Controller, "GetUser returned: {Data}", result.Data);
            return Ok(result.Serialize());
        }

        [HttpPost("{userName}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddUser(string userName)
        {
            logger.LogDebug(AppLogEvents.Controller, "Invoked AddUser with data: {user}", userName);

            var result = await userService.CreateUser(new User() { Username = userName, CreationDate = DateTime.Now});
            logger.LogDebug(AppLogEvents.Controller, "CreateUser result: {result}", result);

            if (result.Success)
            {
                logger.LogInformation(AppLogEvents.Controller, "AddUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            logger.LogWarning(AppLogEvents.Controller, "AddUser returned error(s): {Errors}", result.Errors);
            return Problem("User already exist or sth else went wrong", statusCode: 400);
        }
    }
}