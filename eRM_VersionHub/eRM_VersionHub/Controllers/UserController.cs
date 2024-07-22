using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ApiResponse<List<User>> result = await _userService.GetUserList();
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetUser(string Username)
        {
            ApiResponse<User?> result = await _userService.GetUser(Username);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            ApiResponse<User?> result = await _userService.CreateUser(user);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            ApiResponse<User?> result = await _userService.UpdateUser(user);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete("{Username}")]
        public async Task<IActionResult> DeleteUser(string Username)
        {
            ApiResponse<User?> result = await _userService.DeleteUser(Username);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }
    }
}