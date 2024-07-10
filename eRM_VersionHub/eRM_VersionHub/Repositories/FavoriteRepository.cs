using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Repositories.Database
{
    public class FavoriteRepository(IDbRepository dbRepository) : IFavoriteRepository
    {
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<Result<Favorite?>> CreateFavorite(Favorite favorite)
        {
            Result<Favorite?> CreatedFavorite = await _dbRepository.EditData<Favorite>(
                "INSERT INTO favorites(username, app_id) VALUES (@Username, @AppID) RETURNING *", favorite);
            return CreatedFavorite;
        }

        public async Task<Result<List<Favorite>>> GetFavoriteList(string Username)
        {
            Result<List<Favorite>> FavoriteList = await _dbRepository.GetAll<Favorite>(
                "SELECT * FROM favorites WHERE username=@Username", new { Username });
            return FavoriteList;
        }

        public async Task<Result<Favorite?>> DeleteFavorite(Favorite favorite)
        {
            Result<Favorite?> DeletedFavorite = await _dbRepository.EditData<Favorite>(
                "DELETE FROM favorites WHERE username=@Username AND app_id=@AppID RETURNING *", favorite);
            return DeletedFavorite;
        }
    }
}