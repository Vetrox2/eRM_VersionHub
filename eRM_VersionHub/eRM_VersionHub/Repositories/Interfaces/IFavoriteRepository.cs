using eRM_VersionHub.Models;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Result<Favorite?>> CreateFavorite(Favorite favorite);
        Task<Result<List<Favorite>>> GetFavoriteList(string Username);
        Task<Result<Favorite?>> DeleteFavorite(Favorite favorite);
    }
}