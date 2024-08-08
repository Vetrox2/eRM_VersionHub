using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<Favorite> CreateFavorite(Favorite favorite);
        Task<List<Favorite>> GetFavoriteList(string Username);
        Task<Favorite> DeleteFavorite(Favorite favorite);
    }
}
