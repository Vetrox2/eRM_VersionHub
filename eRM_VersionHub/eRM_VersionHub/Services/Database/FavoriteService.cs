using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Result;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class FavoriteService(IFavoriteRepository repository) : IFavoriteService
    {
        private readonly IFavoriteRepository _repository = repository;

        public async Task<Result<Favorite?>> CreateFavorite(Favorite favorite)
        {
            return await _repository.CreateFavorite(favorite);
        }

        public async Task<Result<List<Favorite>>> GetFavoriteList(string Username)
        {
            return await _repository.GetFavoriteList(Username);
        }

        public async Task<Result<Favorite?>> DeleteFavorite(Favorite favorite)
        {
            return await _repository.DeleteFavorite(favorite);
        }
    }
}
