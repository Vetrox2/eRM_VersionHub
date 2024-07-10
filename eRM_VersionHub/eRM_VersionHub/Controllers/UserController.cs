using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using Microsoft.AspNetCore.Mvc;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Result<List<User>> result = await _userService.GetUserList();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUser(string Username)
        {
            Result<User?> result = await _userService.GetUser(Username);
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
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            Result<User?> result = await _userService.CreateUser(user);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            Result<User?> result = await _userService.UpdateUser(user);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpDelete("{Username}")]
        public async Task<IActionResult> DeleteUser(string Username)
        {
            Result<User?> result = await _userService.DeleteUser(Username);
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