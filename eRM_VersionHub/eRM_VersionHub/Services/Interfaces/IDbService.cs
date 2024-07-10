using eRM_VersionHub.Result;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IDbService
    {
        Task<Result<T?>> GetAsync<T>(string command, object parms);
        Task<Result<List<T>>> GetAll<T>(string command, object parms);
        Task<Result<T?>> EditData<T>(string command, object parms);
    }
}