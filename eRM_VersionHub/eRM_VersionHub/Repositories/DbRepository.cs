using Dapper;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Result;
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

        public async Task<Result<bool>> CreateTable(
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
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(
                    new List<string> { $"Error creating table: {ex.Message}" }
                );
            }
        }

        public async Task<Result<bool>> TableExists(string tableName)
        {
            var sql =
                @"SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_name = @TableName
                        )";
            var result = await _db.ExecuteScalarAsync<bool>(sql, new { TableName = tableName });
            if (result == true)
            {
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure(["Table dont exist"]);
        }

        public async Task<Result<T?>> GetAsync<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                if (result == null)
                {
                    return Result<T?>.Failure(
                        ["Object was not found"],
                        "There is no objects that satisfy this criteria",
                        404
                    );
                }
                return Result<T?>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T?>.Failure([ex.Message], ex.StackTrace, 500);
            }
        }

        public async Task<Result<List<T>>> GetAll<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                List<T> result = query.ToList();
                if (result == null)
                {
                    return Result<List<T>>.Failure(
                        ["No data available"],
                        "There is no data in this table",
                        404
                    );
                }
                return Result<List<T>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<T>>.Failure([ex.Message], ex.StackTrace, 500);
            }
        }

        public async Task<Result<T?>> EditData<T>(string command, object parms)
        {
            try
            {
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();
                if (result == null)
                {
                    return Result<T?>.Failure(
                        ["Object was not found"],
                        "There is no objects that satisfy this criteria",
                        404
                    );
                }
                return Result<T?>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<T?>.Failure([ex.Message], ex.StackTrace, 500);
            }
        }
    }
}
