namespace eRM_VersionHub.Services.Interfaces
{
    public interface IDbService
    {
        Task<T?> GetAsync<T>(string template, object parameters);
        Task<List<T>> GetAll<T>(string template, object parameters);
        Task<T?> EditData<T>(string template, object parameters);
    }
}
