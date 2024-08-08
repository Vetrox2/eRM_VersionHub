using eRM_VersionHub.Models;

public interface IFavoriteRepository
{
    Task<Favorite> CreateFavorite(Favorite favorite);
    Task<List<Favorite>> GetFavoriteList(string Username);
    Task<Favorite> DeleteFavorite(Favorite favorite);
}
