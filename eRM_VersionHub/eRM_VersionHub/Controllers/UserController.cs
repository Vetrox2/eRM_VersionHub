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
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpGet("UsersWithApps")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersWithApps()
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetUsersWithApps");
            var result = await _userService.GetUsersWithApps();

            _logger.LogDebug(AppLogEvents.Controller, "GetUsersWithApps result: {result}", result);

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning(AppLogEvents.Controller, "GetUser returned error(s): {Errors}", result.Errors);
                return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
            }

            _logger.LogInformation(AppLogEvents.Controller, "GetUser returned: {Data}", result.Data);
            return Ok(result.Serialize());
        }
        [HttpGet()]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetAllUsers");
            var result = await _userService.GetUserNamesList();

            if (!result.Success || result.Data == null)
            {
                _logger.LogWarning(AppLogEvents.Controller, "GetUser returned error(s): {Errors}", result.Errors);
                return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
            }

            return Ok(result.Serialize());
        }

        [HttpPost("{userName}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddUser(string userName)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked AddUser with data: {user}", userName);
            ApiResponse<User?> result = await _userService.CreateUser(new User() { Username = userName, CreationDate = DateTime.Now });
            _logger.LogDebug(AppLogEvents.Controller, "CreateUser result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "AddUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "AddUser returned error(s): {Errors}", result.Errors);
            return Problem("User already exist or sth else went wrong", statusCode: 400);
        }
    }
}