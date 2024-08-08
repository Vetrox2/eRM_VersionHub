using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoritesService;
        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(
            IFavoriteService favoritesService,
            ILogger<FavoriteController> logger
        )
        {
            _favoritesService = favoritesService;
            _logger = logger;
        }

        [HttpGet("{Username}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetFavorites(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked GetFavorites with parameter: {Username}",
                Username
            );
            var result = await _favoritesService.GetFavoriteList(Username);
            _logger.LogInformation(
                AppLogEvents.Controller,
                "GetFavorites returned: {Data}",
                result
            );
            return Ok(ApiResponse<List<Favorite>>.SuccessResponse(result).Serialize());
        }

        [HttpPost("{UserName}/{app_id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddFavorite(string UserName, string app_id)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked AddToFavorites with parameters: {UserName}, {app_id}",
                UserName,
                app_id
            );
            var result = await _favoritesService.CreateFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            _logger.LogInformation(
                AppLogEvents.Controller,
                "AddToFavorites returned: {Data}",
                result
            );
            return Ok(ApiResponse<Favorite>.SuccessResponse(result).Serialize());
        }

        [HttpDelete("{UserName}/{app_id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteFavorite(string UserName, string app_id)
        {
            _logger.LogDebug(
                AppLogEvents.Controller,
                "Invoked RemoveFromFavorites with parameters: {UserName}, {app_id}",
                UserName,
                app_id
            );
            var result = await _favoritesService.DeleteFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            _logger.LogInformation(
                AppLogEvents.Controller,
                "RemoveFromFavorites returned: {Data}",
                result
            );
            return Ok(ApiResponse<Favorite>.SuccessResponse(result).Serialize());
        }
    }
}
