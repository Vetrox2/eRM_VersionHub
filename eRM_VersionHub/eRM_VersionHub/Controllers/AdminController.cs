using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "user")]
        public IActionResult IsAdmin()
        {
            if (User == null)
                return NotFound(ApiResponse<string>.ErrorResponse(["User not found"]).Serialize());

            var username = User.Identity?.Name ?? "Username unknown";

            return User.IsInRole("admin") ? 
                Ok(ApiResponse<string>.SuccessResponse($"User {username} is an admin").Serialize()) :
                StatusCode(403, ApiResponse<string>.ErrorResponse([$"User {username} is not an admin"]).Serialize());
        }
    }
}
