using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoriteController(IFavoriteService favoritesService) : ControllerBase
    {
        private readonly IFavoriteService _favoritesService = favoritesService;

        [HttpGet("{Username}")]
        public async Task<IActionResult> GetFavorites(string Username)
        {
            Result<List<Favorite>> result = await _favoritesService.GetFavoriteList(Username);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpPost("{UserName}/{app_id}")]
        public async Task<IActionResult> AddToFavorites(string UserName, string app_id)
        {
            var result = await _favoritesService.CreateFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpDelete("{UserName}/{app_id}")]
        public async Task<IActionResult> RemoveFromFavorites(string UserName, string app_id)
        {
            var result = await _favoritesService.DeleteFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
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
        public async Task<IActionResult> AddFavorites([FromBody] Favorite favorite)
        {
            Result<Favorite?> result = await _favoritesService.CreateFavorite(favorite);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return Problem(
                detail: string.Join(";", result.ErrorMessages),
                statusCode: result.ProblemDetails.Status
            );
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] Favorite favorite)
        {
            Result<Favorite?> result = await _favoritesService.DeleteFavorite(favorite);
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
