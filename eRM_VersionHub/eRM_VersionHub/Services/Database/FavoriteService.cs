using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class FavoriteService(IFavoriteRepository repository, ILogger<FavoriteService> logger) : IFavoriteService
    {
        private readonly ILogger<FavoriteService> _logger = logger;
        private readonly IFavoriteRepository _repository = repository;

        public async Task<ApiResponse<Favorite?>> CreateFavorite(Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked CreateFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _repository.CreateFavorite(favorite);
            _logger.LogInformation(AppLogEvents.Service, "CreateFavorite returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<Favorite>>> GetFavoriteList(string Username)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetFavoriteList with parameter: {Username}", Username);
            ApiResponse<List<Favorite>> result = await _repository.GetFavoriteList(Username);
            _logger.LogInformation(AppLogEvents.Service, "GetFavoriteList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<Favorite?>> DeleteFavorite(Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked DeleteFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _repository.DeleteFavorite(favorite);
            _logger.LogInformation(AppLogEvents.Service, "DeleteFavorite returned: {result}", result);
            return result;
        }
    }
}
