using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IDbService
    {
        Task<ApiResponse<T?>> GetAsync<T>(string command, object parms);
        Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms);
        Task<ApiResponse<T?>> EditData<T>(string command, object parms);
    }
}