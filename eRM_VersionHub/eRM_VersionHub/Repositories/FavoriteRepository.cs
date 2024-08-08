using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ILogger<FavoriteRepository> _logger;
    private readonly IDbRepository _dbRepository;

    public FavoriteRepository(IDbRepository dbRepository, ILogger<FavoriteRepository> logger)
    {
        _logger = logger;
        _dbRepository = dbRepository;
    }

    public async Task<Favorite> CreateFavorite(Favorite favorite)
    {
        _logger.LogDebug(
            AppLogEvents.Repository,
            "Invoked CreateFavorite with data: {favorite}",
            favorite
        );
        var result = await _dbRepository.EditData<Favorite>(
            "INSERT INTO favorites(username, app_id) VALUES (@Username, @AppID) RETURNING *",
            favorite
        );
        if (result == null)
        {
            throw new InvalidOperationException("Failed to create favorite");
        }
        _logger.LogInformation(AppLogEvents.Repository, "CreateFavorite completed successfully");
        return result;
    }

    public async Task<List<Favorite>> GetFavoriteList(string Username)
    {
        _logger.LogDebug(
            AppLogEvents.Repository,
            "Invoked GetFavoriteList with parameter: {Username}",
            Username
        );
        var result = await _dbRepository.GetAll<Favorite>(
            "SELECT * FROM favorites WHERE username=@Username",
            new { Username }
        );
        if (result == null || !result.Any())
        {
            throw new NotFoundException($"No favorites found for user {Username}");
        }
        _logger.LogInformation(AppLogEvents.Repository, "GetFavoriteList completed successfully");
        return result;
    }

    public async Task<Favorite> DeleteFavorite(Favorite favorite)
    {
        _logger.LogDebug(
            AppLogEvents.Repository,
            "Invoked DeleteFavorite with data: {favorite}",
            favorite
        );
        var result = await _dbRepository.EditData<Favorite>(
            "DELETE FROM favorites WHERE username=@Username AND app_id=@AppID RETURNING *",
            favorite
        );
        if (result == null)
        {
            throw new NotFoundException("Favorite not found");
        }
        _logger.LogInformation(AppLogEvents.Repository, "DeleteFavorite completed successfully");
        return result;
    }
}
