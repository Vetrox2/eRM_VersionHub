using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;


namespace eRM_VersionHub.Repositories
{
    public class FavoriteRepository(IDbRepository dbRepository, ILogger<FavoriteRepository> logger) : IFavoriteRepository
    {
        private readonly ILogger<FavoriteRepository> _logger = logger;
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<ApiResponse<Favorite?>> CreateFavorite(Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked CreateFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _dbRepository.EditData<Favorite>(
                "INSERT INTO favorites(username, app_id) VALUES (@Username, @AppID) RETURNING *", favorite);

            _logger.LogInformation(AppLogEvents.Repository, "CreateFavorite returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<Favorite>>> GetFavoriteList(string Username)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetFavoriteList with parameter: {Username}", Username);
            ApiResponse<List<Favorite>> result = await _dbRepository.GetAll<Favorite>(
                "SELECT * FROM favorites WHERE username=@Username", new { Username });

            _logger.LogInformation(AppLogEvents.Repository, "GetFavoriteList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<Favorite?>> DeleteFavorite(Favorite favorite)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked DeleteFavorite with data: {favorite}", favorite);
            ApiResponse<Favorite?> result = await _dbRepository.EditData<Favorite>(
                "DELETE FROM favorites WHERE username=@Username AND app_id=@AppID RETURNING *", favorite);

            _logger.LogInformation(AppLogEvents.Repository, "DeleteFavorite returned: {result}", result);
            return result;
        }
    }
}