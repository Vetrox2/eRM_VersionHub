using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ILogger<FavoriteService> _logger;
        private readonly IFavoriteRepository _repository;

        public FavoriteService(IFavoriteRepository repository, ILogger<FavoriteService> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<Favorite> CreateFavorite(Favorite favorite)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked CreateFavorite with data: {favorite}",
                favorite
            );

            var result = await _repository.CreateFavorite(favorite);

            if (result == null)
            {
                throw new BadRequestException("Failed to create favorite");
            }

            _logger.LogInformation(
                AppLogEvents.Service,
                "CreateFavorite returned: {result}",
                result
            );

            return result;
        }

        public async Task<List<Favorite>> GetFavoriteList(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked GetFavoriteList with parameter: {Username}",
                Username
            );

            var result = await _repository.GetFavoriteList(Username);

            if (result == null || !result.Any())
            {
                throw new NotFoundException($"No favorites found for user: {Username}");
            }

            _logger.LogInformation(
                AppLogEvents.Service,
                "GetFavoriteList returned: {result}",
                result
            );

            return result;
        }

        public async Task<Favorite> DeleteFavorite(Favorite favorite)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked DeleteFavorite with data: {favorite}",
                favorite
            );

            var result = await _repository.DeleteFavorite(favorite);

            if (result == null)
            {
                throw new NotFoundException("Favorite not found or could not be deleted");
            }

            _logger.LogInformation(
                AppLogEvents.Service,
                "DeleteFavorite returned: {result}",
                result
            );

            return result;
        }
    }
}
