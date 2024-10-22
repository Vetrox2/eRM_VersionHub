﻿using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "admin")]
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

        [HttpPost("{UserName}/{app_id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddFavorite(string UserName, string app_id)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked AddToFavorites with paramters: {UserName}, {app_id}", UserName, app_id);
            var result = await _favoritesService.CreateFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            _logger.LogDebug(AppLogEvents.Controller, "CreateFavorite result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "AddToFavorites returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "AddToFavorites returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors), statusCode: 400);
        }

        [HttpDelete("{UserName}/{app_id}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> DeleteFavorite(string UserName, string app_id)
        {
            _logger.LogDebug(AppLogEvents.Controller, "Invoked RemoveFromFavorites with paramters: {UserName}, {app_id}", UserName, app_id);
            var result = await _favoritesService.DeleteFavorite(
                new Favorite { Username = UserName, AppID = app_id }
            );
            _logger.LogDebug(AppLogEvents.Controller, "DeleteFavorite result: {result}", result);

            if (result.Success)
            {
                _logger.LogInformation(AppLogEvents.Controller, "RemoveFromFavorites returned: {Data}", result.Data);
                return Ok(result.Data);
            }

            _logger.LogWarning(AppLogEvents.Controller, "RemoveFromFavorites returned error(s): {Errors}", result.Errors);
            return Problem(detail: string.Join(";", result.Errors));
        }
    }
}
