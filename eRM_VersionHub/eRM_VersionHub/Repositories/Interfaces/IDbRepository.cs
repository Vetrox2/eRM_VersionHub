using static eRM_VersionHub.Repositories.DbRepository;

public interface IDbRepository
{
    Task<bool> CreateTable(string tableName, List<ColumnDefinition> columns);
    Task<bool> TableExists(string tableName);
    Task<T?> GetAsync<T>(string command, object parms);
    Task<List<T>> GetAll<T>(string command, object parms);
    Task<T?> EditData<T>(string command, object parms);
}
