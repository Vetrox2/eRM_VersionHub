using eRM_VersionHub.Models;
using static eRM_VersionHub.Repositories.DbRepository;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IDbRepository
    {
        Task<ApiResponse<T?>> GetAsync<T>(string command, object parms);
        Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms);
        Task<ApiResponse<T?>> EditData<T>(string command, object parms);
        Task<ApiResponse<bool>> TableExists(string tableName);
        Task<ApiResponse<bool>> CreateTable(string tableName, List<ColumnDefinition> columns);
    }
}
