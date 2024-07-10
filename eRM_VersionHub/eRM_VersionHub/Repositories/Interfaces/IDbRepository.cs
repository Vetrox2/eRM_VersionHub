using eRM_VersionHub.Result;
using static eRM_VersionHub.Repositories.Database.DbRepository;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IDbRepository
    {
        Task<Result<T?>> GetAsync<T>(string command, object parms);
        Task<Result<List<T>>> GetAll<T>(string command, object parms);
        Task<Result<T?>> EditData<T>(string command, object parms);
        Task<Result<bool>> TableExists(string tableName);
        Task<Result<bool>> CreateTable(string tableName, List<ColumnDefinition> columns);
    }
}
