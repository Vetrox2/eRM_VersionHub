using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace eRM_VersionHub.Services.Database
{
    public class SqlConnectionFactory(IOptions<AppSettings> appSettings) : ISqlConnectionFactory
    {
        private readonly string _connectionString = appSettings.Value.MyAppSettings.ConnectionString;

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
