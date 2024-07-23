using eRM_VersionHub.Models;

using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteController(IFavoriteService favoritesService, ILogger<FavoriteController> logger) : ControllerBase
    {
        private readonly ILogger<FavoriteController> _logger = logger;
        private readonly IFavoriteService _favoritesService = favoritesService;

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetFavorites(string Username)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked GetFavorites with paramter: {Username}", Username);
            ApiResponse<List<Favorite>> result = await _favoritesService.GetFavoriteList(Username);
            _logger.LogDebug(AppLogEvents.Controller, "GetFavoriteList result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "GetFavorites returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "GetFavorites returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked AddFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _favoritesService.CreateFavorite(favorite);
            _logger.LogDebug(AppLogEvents.Controller, "CreateFavorite result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "AddFavorite returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "AddFavorite returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFavorite([FromBody] Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked DeleteFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _favoritesService.DeleteFavorite(favorite);
            _logger.LogDebug(AppLogEvents.Controller, "DeleteFavorite result: {result}", result);
            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "DeleteFavorite returned: {Data}", result.Data);
                return Ok(result.Data);
            }
            _logger.LogWarning(AppLogEvents.Controller, "DeleteFavorite returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpPost("{UserName}/{app_id}")]
        public async Task<IActionResult> AddToFavorites(string UserName, string app_id)
        {
            var result = await _favoritesService.CreateFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete("{UserName}/{app_id}")]
        public async Task<IActionResult> RemoveFromFavorites(string UserName, string app_id)
        {
            var result = await _favoritesService.DeleteFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }
    }
}
