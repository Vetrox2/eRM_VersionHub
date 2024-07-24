using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetUsers");
            ApiResponse<List<User>> result = await _userService.GetUserList();
            _logger.LogDebug(AppLogEvents.Controller, "GetUserList result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "GetUsers returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "GetUsers returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetUser with parameter: {Username}", Username);
            ApiResponse<User?> result = await _userService.GetUser(Username);
            _logger.LogDebug(AppLogEvents.Controller, "GetUser result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "GetUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "GetUser returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked AddUser with data: {user}", user);
            ApiResponse<User?> result = await _userService.CreateUser(user);
            _logger.LogDebug(AppLogEvents.Controller, "CreateUser result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "AddUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "AddUser returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked UpdateUser with data: {user}", user);
            ApiResponse<User?> result = await _userService.UpdateUser(user);
            _logger.LogDebug(AppLogEvents.Controller, "UpdateUser result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "UpdateUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "UpdateUser returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete("{Username}")]
        public async Task<IActionResult> DeleteUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked DeleteUser with parameter: {Username}", Username);
            ApiResponse<User?> result = await _userService.DeleteUser(Username);
            _logger.LogDebug(AppLogEvents.Controller, "DeleteUser result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "DeleteUser returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "DeleteUser returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }
    }
}