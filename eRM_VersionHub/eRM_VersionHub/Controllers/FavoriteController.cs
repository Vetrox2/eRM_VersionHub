﻿using eRM_VersionHub.Models;

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
            ApiResponse<List<Favorite>> result = await _favoritesService.GetFavoriteList(Username);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
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
            return Problem(detail: string.Join(";", result.Errors));
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

        [HttpPost]
        public async Task<IActionResult> AddFavorites([FromBody] Favorite favorite)
        {
            ApiResponse<Favorite?> result = await _favoritesService.CreateFavorite(favorite);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission([FromBody] Favorite favorite)
        {
            ApiResponse<Favorite?> result = await _favoritesService.DeleteFavorite(favorite);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return Problem(detail: string.Join(";", result.Errors));
        }
    }
}
