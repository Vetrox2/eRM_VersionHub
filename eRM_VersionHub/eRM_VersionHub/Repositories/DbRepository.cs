using Dapper;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Text.RegularExpressions;

namespace eRM_VersionHub.Repositories
{
    public class DbRepository(ILogger<DbRepository> logger, ISqlConnectionFactory connectionFactory) : IDbRepository
    {
        private readonly ILogger _logger = logger;


        public class ColumnDefinition
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool PrimaryKey { get; set; }
            public bool NotNull { get; set; }
            public bool Unique { get; set; }

            public override string ToString() => $"{Name} {Type} {(PrimaryKey ? " PRIMARY KEY" : "")} {(NotNull ? "NOT NULL" : "")} {(Unique ? " UNIQUE" : "")}";
        }
        private static readonly Regex ValidIdentifierRegex = new Regex(@"^[a-zA-Z0-9_]+$");


        private static bool IsValidIdentifier(string identifier)
        {
            return ValidIdentifierRegex.IsMatch(identifier);
        }

        private static string SanitizeIdentifier(string identifier)
        {
            // Double quote the identifier to escape it properly for SQL
            return $"\"{identifier.Replace("\"", "\"\"")}\"";
        }



        public async Task<ApiResponse<bool>> CreateTable(string tableName, List<ColumnDefinition> columns)
        {
            _logger.LogDebug(AppLogEvents.Database, "Invoked CreateTable with data: {tableName}\n{columns}", tableName, columns);

            if (!IsValidIdentifier(tableName))
            {
                return ApiResponse<bool>.ErrorResponse(new List<string> { "Invalid table name." });
            }

            try
            {
                using var _db = connectionFactory.CreateConnection();


                var columnDefinitions = columns.Select(c => c.ToString());
                var allColumns = string.Join(",\n", columnDefinitions);
                var sql = $"CREATE TABLE IF NOT EXISTS {SanitizeIdentifier(tableName)} ({allColumns});";

                _logger.LogDebug(AppLogEvents.Database, "CreateTable is executing an SQL query: {sql}, ({tableName}, {columnDefinitions})", sql, tableName, columnDefinitions);
                await _db.ExecuteAsync(sql);

                _logger.LogInformation(AppLogEvents.Database, "CreateTable invoked successfully");


                return ApiResponse<bool>.SuccessResponse(true);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Database, "While executing CreateTable, this exception has been thrown: {Message}\n{StackTrace}",
                    ex.Message, ex.StackTrace);

                return ApiResponse<bool>.ErrorResponse(new List<string> { $"Error creating table: {ex.Message}" });
            }
        }


        public async Task<ApiResponse<bool>> TableExists(string tableName)
        {
            _logger.LogDebug(AppLogEvents.Database, "Invoked TableExists with parameter: {tableName}", tableName);
            var sql = "SELECT 1 FROM information_schema.tables WHERE table_name = @TableName";

            using var _db = connectionFactory.CreateConnection();

            _logger.LogDebug(AppLogEvents.Database, "TableExists is executing an SQL query: {sql}", sql);
            var result = await _db.ExecuteScalarAsync<bool>(sql, new { TableName = tableName });
            _logger.LogDebug(AppLogEvents.Database, "Result of query from function TableExists: {result}", result);

            if (result == true)
            {
                _logger.LogInformation(AppLogEvents.Database, "TableExists invoked succesfully");
                return ApiResponse<bool>.SuccessResponse(true);
            }


            _logger.LogWarning(AppLogEvents.Database, "Table {tableName} doesn't exist", tableName);
            return ApiResponse<bool>.ErrorResponse(["Table doesn't exist"]);
        }

        public async Task<ApiResponse<T?>> GetAsync<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAsync of type {type} with query: {command}, {parms}", type, command, parms);

            try
            {
                _logger.LogDebug(AppLogEvents.Database, "GetAsync of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);

                using var _db = connectionFactory.CreateConnection();
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();

                _logger.LogDebug(AppLogEvents.Database, "Result of GetAsync of type {type}: {result}", type, query);
                if (result == null)
                {
                    _logger.LogWarning(AppLogEvents.Database, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }

                _logger.LogInformation(AppLogEvents.Database, "GetAsync of type {type} returned: {result}", type, result);
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Database, "While executing GetAsync of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                   type, ex.Message, ex.StackTrace);
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAll of type {type} with query: {command}, {parms}", type, command, parms);

            try
            {
                _logger.LogDebug(AppLogEvents.Database, "GetAll of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);

                using var _db = connectionFactory.CreateConnection();
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                List<T> result = query.ToList();

                _logger.LogDebug(AppLogEvents.Database, "Result of GetAll of type {type}: {result}", type, query);
                if (result == null)
                {
                    _logger.LogWarning(AppLogEvents.Database, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<List<T>>.ErrorResponse(["No data available"]);
                }

                _logger.LogInformation(AppLogEvents.Database, "GetAll of type {type} returned: {result}", type, result);
                return ApiResponse<List<T>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Database, "While executing GetAll of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                    type, ex.Message, ex.StackTrace);
                return ApiResponse<List<T>>.ErrorResponse([ex.Message]);
            }
        }

        public async Task<ApiResponse<T?>> EditData<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked EditData of type {type} with query: {command}, {parms}", type, command, parms);

            try
            {
                _logger.LogDebug(AppLogEvents.Database, "EditData of type {type} is executing an SQL query: {command}, {parms}", type, command, parms);

                using var _db = connectionFactory.CreateConnection();
                IEnumerable<T> query = await _db.QueryAsync<T>(command, parms);
                T? result = query.FirstOrDefault();

                _logger.LogDebug(AppLogEvents.Database, "Result of EditData of type {type}: {result}", type, query);
                if (result == null)
                {
                    _logger.LogWarning(AppLogEvents.Database, "This query returned null: {command}, {parms}", command, parms);
                    return ApiResponse<T?>.ErrorResponse(["Object was not found"]);
                }

                _logger.LogInformation(AppLogEvents.Database, "EditData of type {type} returned: {result}", type, result);
                return ApiResponse<T?>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(AppLogEvents.Database, "While executing EditData of type {type}, this execption has been thrown: {Message}\n{StackTrace}",
                    type, ex.Message, ex.StackTrace);
                return ApiResponse<T?>.ErrorResponse([ex.Message]);
            }
        }
    }
}
