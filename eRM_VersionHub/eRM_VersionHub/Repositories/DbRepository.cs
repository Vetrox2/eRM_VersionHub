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

        public DbRepository(IOptions<AppSettings> appSettings)
        {
            configuration = appSettings.Value.MyAppSettings.ConnectionString;
            _db = new NpgsqlConnection(configuration);
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

                await _db.ExecuteAsync(sql);
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    new List<string> { $"Error creating table: {ex.Message}" }
                );
            }
        }

        public async Task<ApiResponse<bool>> TableExists(string tableName)
        {
            var sql =
                @"SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_name = @TableName
                        )";
            var result = await _db.ExecuteScalarAsync<bool>(sql, new { TableName = tableName });
            if (result == true)
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }
            return ApiResponse<bool>.ErrorResponse(["Table dont exist"]);
        }

        public async Task<ApiResponse<T?>> GetAsync<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                if (result == null)
                {
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                List<T> result = query.ToList();
                if (result == null)
                {
                    return ApiResponse<List<T>>.ErrorResponse(["No data available"]);
                }
                return ApiResponse<List<T>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<T>>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<T?>> EditData<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                if (result == null)
                {
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }
    }
}
