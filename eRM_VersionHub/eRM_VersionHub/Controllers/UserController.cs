using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("UsersWithApps")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsersWithApps()
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetUsersWithApps");
            var result = await _userService.GetUsersWithApps();
            _logger.LogInformation(
                AppLogEvents.Controller,
                "GetUsersWithApps returned: {Data}",
                result
            );
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetAllUsers");
            var result = await _userService.GetUserNamesList();
            _logger.LogInformation(AppLogEvents.Controller, "GetAllUsers returned: {Data}", result);
            return Ok(ApiResponse<List<string>>.SuccessResponse(result).Serialize());
        }

        [HttpPost("{userName}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddUser(string userName)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked AddUser with data: {user}",
                userName
            );
            var result = await _userService.CreateUser(
                new User { Username = userName, CreationDate = DateTime.Now }
            );
            _logger.LogInformation(AppLogEvents.Controller, "AddUser returned: {Data}", result);
            return Ok(result);
        }
    }
}
