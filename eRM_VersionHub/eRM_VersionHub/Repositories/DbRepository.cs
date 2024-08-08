using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class DbRepository : IDbRepository
    {
        private readonly ILogger<DbRepository> _logger;
        private readonly ISqlConnectionFactory _connectionFactory;

        public DbRepository(ILogger<DbRepository> logger, ISqlConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public class ColumnDefinition
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public bool PrimaryKey { get; set; }
            public bool NotNull { get; set; }
            public bool Unique { get; set; }

            public override string ToString() =>
                $"{Name} {Type} {(PrimaryKey ? " PRIMARY KEY" : "")} {(NotNull ? "NOT NULL" : "")} {(Unique ? " UNIQUE" : "")}";
        }

        private static readonly Regex ValidIdentifierRegex = new Regex(@"^[a-zA-Z0-9_]+$");

        private static bool IsValidIdentifier(string identifier)
        {
            return ValidIdentifierRegex.IsMatch(identifier);
        }

        private static string SanitizeIdentifier(string identifier)
        {
            return $"\"{identifier.Replace("\"", "\"\"")}\"";
        }

        public async Task<bool> CreateTable(string tableName, List<ColumnDefinition> columns)
        {
            _logger.LogDebug(
                AppLogEvents.Database,
                "Invoked CreateTable with data: {tableName}\n{columns}",
                tableName,
                columns
            );

            if (!IsValidIdentifier(tableName))
            {
                throw new ArgumentException("Invalid table name.");
            }

            try
            {
                using var _db = _connectionFactory.CreateConnection();

                var columnDefinitions = columns.Select(c => c.ToString());
                var allColumns = string.Join(",\n", columnDefinitions);
                var sql =
                    $"CREATE TABLE IF NOT EXISTS {SanitizeIdentifier(tableName)} ({allColumns});";

                _logger.LogDebug(
                    AppLogEvents.Database,
                    "CreateTable is executing an SQL query: {sql}, ({tableName}, {columnDefinitions})",
                    sql,
                    tableName,
                    columnDefinitions
                );
                await _db.ExecuteAsync(sql);

                _logger.LogInformation(AppLogEvents.Database, "CreateTable invoked successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error creating table: {tableName}",
                    tableName
                );
                throw new InvalidOperationException($"Error creating table: {ex.Message}", ex);
            }
        }

        public async Task<bool> TableExists(string tableName)
        {
            _logger.LogDebug(
                AppLogEvents.Database,
                "Invoked TableExists with parameter: {tableName}",
                tableName
            );
            var sql = "SELECT 1 FROM information_schema.tables WHERE table_name = @TableName";

            using var _db = _connectionFactory.CreateConnection();

            _logger.LogDebug(
                AppLogEvents.Database,
                "TableExists is executing an SQL query: {sql}",
                sql
            );
            var result = await _db.ExecuteScalarAsync<bool>(sql, new { TableName = tableName });
            _logger.LogDebug(
                AppLogEvents.Database,
                "Result of query from function TableExists: {result}",
                result
            );

            if (result)
            {
                _logger.LogInformation(AppLogEvents.Database, "TableExists invoked successfully");
                return true;
            }

            _logger.LogWarning(AppLogEvents.Database, "Table {tableName} doesn't exist", tableName);
            return false;
        }

        public async Task<T?> GetAsync<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(
                AppLogEvents.Database,
                "Invoked GetAsync of type {type} with query: {command}, {parms}",
                type,
                command,
                parms
            );

            try
            {
                using var _db = _connectionFactory.CreateConnection();
                var result = await _db.QueryFirstOrDefaultAsync<T>(command, parms);

                if (result == null)
                {
                    _logger.LogWarning(
                        AppLogEvents.Database,
                        "This query returned null: {command}, {parms}",
                        command,
                        parms
                    );
                    throw new NotFoundException($"Object of type {type} was not found");
                }

                _logger.LogInformation(
                    AppLogEvents.Database,
                    "GetAsync of type {type} returned successfully",
                    type
                );
                return result;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error executing GetAsync of type {type}",
                    type
                );
                throw new InvalidOperationException($"Error executing query: {ex.Message}", ex);
            }
        }

        public async Task<List<T>> GetAll<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(
                AppLogEvents.Database,
                "Invoked GetAll of type {type} with query: {command}, {parms}",
                type,
                command,
                parms
            );

            try
            {
                using var _db = _connectionFactory.CreateConnection();
                var result = (await _db.QueryAsync<T>(command, parms)).ToList();

                if (result == null || !result.Any())
                {
                    _logger.LogWarning(
                        AppLogEvents.Database,
                        "This query returned no data: {command}, {parms}",
                        command,
                        parms
                    );
                    throw new NotFoundException($"No data available for type {type}");
                }

                _logger.LogInformation(
                    AppLogEvents.Database,
                    "GetAll of type {type} returned successfully",
                    type
                );
                return result;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error executing GetAll of type {type}",
                    type
                );
                throw new InvalidOperationException($"Error executing query: {ex.Message}", ex);
            }
        }

        public async Task<T?> EditData<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(
                AppLogEvents.Database,
                "Invoked EditData of type {type} with query: {command}, {parms}",
                type,
                command,
                parms
            );

            try
            {
                using var _db = _connectionFactory.CreateConnection();
                var result = await _db.QueryFirstOrDefaultAsync<T>(command, parms);

                if (result == null)
                {
                    _logger.LogWarning(
                        AppLogEvents.Database,
                        "This query returned null: {command}, {parms}",
                        command,
                        parms
                    );
                    throw new NotFoundException($"Object of type {type} was not found");
                }

                _logger.LogInformation(
                    AppLogEvents.Database,
                    "EditData of type {type} returned successfully",
                    type
                );
                return result;
            }
            catch (Exception ex) when (ex is not NotFoundException)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error executing EditData of type {type}",
                    type
                );
                throw new InvalidOperationException($"Error executing query: {ex.Message}", ex);
            }
        }
    }
}
