using eRM_VersionHub.Models;


namespace eRM_VersionHub.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<ApiResponse<Favorite?>> CreateFavorite(Favorite favorite);
        Task<ApiResponse<List<Favorite>>> GetFavoriteList(string Username);
        Task<ApiResponse<Favorite?>> DeleteFavorite(Favorite favorite);
    }
}