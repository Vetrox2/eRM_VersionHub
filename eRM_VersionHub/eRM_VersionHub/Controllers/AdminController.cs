﻿using eRM_VersionHub.Models;
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
                return NotFound();

            var username = User.Identity?.Name;
            username = username ?? "ERROR";

            return User.IsInRole("admin") ? 
                Ok(ApiResponse<string>.SuccessResponse($"User {username} is an admin")) :
                NotFound(ApiResponse<string>.ErrorResponse([$"User {username} is not an admin"]));
        }
    }
}
