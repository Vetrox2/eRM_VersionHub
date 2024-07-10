using eRM_VersionHub.Models;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<Result<Favorite?>> CreateFavorite(Favorite favorite);
        Task<Result<List<Favorite>>> GetFavoriteList(string Username);
        Task<Result<Favorite?>> DeleteFavorite(Favorite favorite);
    }
}