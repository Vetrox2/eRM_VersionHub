using Dapper;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace eRM_VersionHub.Repositories.Database
{
    public class DbRepository : IDbRepository
    {
        private static string? configuration;
        private readonly IDbConnection _db;
        private readonly ILogger _logger;

        public DbRepository(IOptions<AppSettings> appSettings, ILogger<DbRepository> logger)
        {
            _logger = logger;
            configuration = appSettings.Value.MyAppSettings.ConnectionString;
            _db = new NpgsqlConnection(configuration);
            _logger.LogDebug(AppLogEvents.Repository, "Started a database connection with settings: {configuration}", configuration);
        }

        public class ColumnDefinition
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool IsPrimaryKey { get; set; }
            public bool IsNullable { get; set; }
            public bool IsUnique { get; set; }
        }

        public async Task<ApiResponse<bool>> CreateTable(
            string tableName,
            List<ColumnDefinition> columns
        )
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked CreateTable with data: {tableName}\n{columns}", tableName, columns);
            try
            {
                var columnDefinitions = columns.Select(c =>
                    $"{c.Name} {c.Type}"
                    + $"{(c.IsPrimaryKey ? " PRIMARY KEY" : "")}"
                    + $"{(c.IsNullable ? " NULL" : " NOT NULL")}"
                    + $"{(c.IsUnique ? " UNIQUE" : "")}"
                );

                var sql =
                    $@"
            CREATE TABLE IF NOT EXISTS {tableName} (
                {string.Join(",\n                ", columnDefinitions)}
            )";
                _logger.LogDebug(AppLogEvents.Repository, "CreateTable is executing an SQL query: {sql}", sql);
                await _db.ExecuteAsync(sql);
                _logger.LogInformation(AppLogEvents.Repository, "CreateTable invoked succesfully");
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Repository,
                    "While executing CreateTable, this execption has been thrown: {Message}\n{StackTrace}",
                    ex.Message, ex.StackTrace);
                return ApiResponse<bool>.ErrorResponse([$"Error creating table: {ex.Message}"]);
            }
        }

        public async Task<ApiResponse<bool>> TableExists(string tableName)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked TableExists with parameter: {tableName}", tableName);
            var sql =
                @"SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_name = @TableName
                        )";
            _logger.LogDebug(AppLogEvents.Repository, "TableExists is executing an SQL query: {sql}", sql);
            var result = await _db.ExecuteScalarAsync<bool>(sql, new { TableName = tableName });
            _logger.LogDebug(AppLogEvents.Repository, "Result of query from function TableExists: {result}", result);
            if (result == true)
            {
                _logger.LogInformation(AppLogEvents.Repository, "TableExists invoked succesfully");
                return ApiResponse<bool>.SuccessResponse(true);
            }
            _logger.LogWarning(AppLogEvents.Repository, "Table {tableName} doesn't exist", tableName);
            return ApiResponse<bool>.ErrorResponse(["Table doesn't exist"]);
        }

        public async Task<ApiResponse<T?>> GetAsync<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetAsync of type {type} with query: {command}, {parms}", type, command, parms);
            try
            {
                _logger.LogDebug(AppLogEvents.Repository, "GetAsync of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                _logger.LogDebug(AppLogEvents.Repository, "Result of GetAsync of type {type}: {result}", type, query);
                if (result == null)
                {
                    _logger.LogWarning(AppLogEvents.Repository, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }
                _logger.LogInformation(AppLogEvents.Repository, "GetAsync of type {type} returned: {result}", type, result);
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                 _logger.LogWarning(AppLogEvents.Repository,
                    "While executing GetAsync of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                    type, ex.Message, ex.StackTrace);
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetAll of type {type} with query: {command}, {parms}", type, command, parms);
            try
            {
                _logger.LogDebug(AppLogEvents.Repository, "GetAll of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                List<T> result = query.ToList();
                _logger.LogDebug(AppLogEvents.Repository, "Result of GetAll of type {type}: {result}", type, query);
                if (result == null)
                {
                     _logger.LogWarning(AppLogEvents.Repository, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<List<T>>.ErrorResponse(["No data available"]);
                }
                _logger.LogInformation(AppLogEvents.Repository, "GetAll of type {type} returned: {result}", type, result);
                return ApiResponse<List<T>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Repository,
                    "While executing GetAll of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                    type, ex.Message, ex.StackTrace);
                return ApiResponse<List<T>>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<T?>> EditData<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Repository, "Invoked EditData of type {type} with query: {command}, {parms}", type, command, parms);
            try
            {
                _logger.LogDebug(AppLogEvents.Repository, "EditData of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                _logger.LogDebug(AppLogEvents.Repository, "Result of EditData of type {type}: {result}", type, query);
                if (result == null)
                {
                    _logger.LogWarning(AppLogEvents.Repository, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }
                _logger.LogInformation(AppLogEvents.Repository, "EditData of type {type} returned: {result}", type, result);
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Repository,
                    "While executing EditData of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                    type, ex.Message, ex.StackTrace);
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }
    }
}
