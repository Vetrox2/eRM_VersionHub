using System.Reflection;
using Dapper;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Database;
using Microsoft.Extensions.Options;
using Moq;
using Npgsql;

namespace eRM_VersionHub.Tests.Repositories
{
    public class DbRepositoryTests : IAsyncLifetime
    {
        private const string TestConnectionString =
            "Host=localhost;Port=5433;Database=testdb;Username=postgres;Password=postgres_test";
        private DbRepository _dbRepository;
        private NpgsqlConnection _realConnection;
        private string _testSchemaName;

        public DbRepositoryTests()
        {
            var mockOptions = new Mock<IOptions<AppSettings>>();
            mockOptions
                .Setup(o => o.Value)
                .Returns(
                    new AppSettings
                    {
                        MyAppSettings = new MyAppSettings
                        {
                            ConnectionString = TestConnectionString
                        }
                    }
                );

            _dbRepository = new DbRepository(mockOptions.Object);
            _testSchemaName = $"test_schema_{Guid.NewGuid():N}";
        }

        public async Task InitializeAsync()
        {
            await EnsureTestDatabaseExists();
            _realConnection = new NpgsqlConnection(TestConnectionString);
            await _realConnection.OpenAsync();
            await CreateTestSchema();
            SetDbRepositoryConnection();
        }

        public async Task DisposeAsync()
        {
            await DropTestSchema();
            await _realConnection.CloseAsync();
            await _realConnection.DisposeAsync();
        }

        private async Task EnsureTestDatabaseExists()
        {
            using var connection = new NpgsqlConnection(TestConnectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync("SELECT 1");
        }

        private async Task CreateTestSchema()
        {
            await _realConnection.ExecuteAsync($"CREATE SCHEMA {_testSchemaName}");
        }

        private async Task DropTestSchema()
        {
            await _realConnection.ExecuteAsync($"DROP SCHEMA {_testSchemaName} CASCADE");
        }

        private void SetDbRepositoryConnection()
        {
            var dbField = typeof(DbRepository).GetField(
                "_db",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            dbField.SetValue(_dbRepository, _realConnection);
        }

        [Fact]
        public async Task CreateTable_ShouldReturnSuccess()
        {
            var tableName = "testtable";
            var tableNameCommand = $"{_testSchemaName}.{tableName}";

            var columnDefinitions = new List<DbRepository.ColumnDefinition>
            {
                new()
                {
                    Name = "Id",
                    Type = "INT",
                    IsPrimaryKey = true,
                    IsNullable = false,
                    IsUnique = true
                },
                new()
                {
                    Name = "Name",
                    Type = "VARCHAR(100)",
                    IsNullable = false
                }
            };

            var result = await _dbRepository.CreateTable(tableNameCommand, columnDefinitions);

            Assert.True(result.Success);
            Assert.True(result.Data);

            var tableExists = await _realConnection.ExecuteScalarAsync<bool>(
                "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_schema = @SchemaName AND table_name = @TableName)",
                new { SchemaName = _testSchemaName, TableName = tableName }
            );
            Assert.True(tableExists);
        }

        [Fact]
        public async Task TableExists_ShouldReturnTrue()
        {
            var tableName = $"{_testSchemaName}.testtable";
            var columnDefinitions = new List<DbRepository.ColumnDefinition>
            {
                new()
                {
                    Name = "Id",
                    Type = "INT",
                    IsPrimaryKey = true,
                    IsNullable = false,
                    IsUnique = true
                }
            };

            await _dbRepository.CreateTable(tableName, columnDefinitions);

            var result = await _dbRepository.TableExists("testtable");

            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnData()
        {
            var tableName = $"{_testSchemaName}.TestTable";
            var columnDefinitions = new List<DbRepository.ColumnDefinition>
            {
                new()
                {
                    Name = "Id",
                    Type = "INT",
                    IsPrimaryKey = true,
                    IsNullable = false,
                    IsUnique = true
                },
                new()
                {
                    Name = "Name",
                    Type = "VARCHAR(100)",
                    IsNullable = false
                }
            };

            await _dbRepository.CreateTable(tableName, columnDefinitions);
            await _realConnection.ExecuteAsync(
                $"INSERT INTO {tableName} (Id, Name) VALUES (@Id, @Name)",
                new { Id = 1, Name = "Test" }
            );

            var result = await _dbRepository.GetAsync<TestModel>(
                $"SELECT * FROM {tableName} WHERE Id = @Id",
                new { Id = 1 }
            );

            Assert.True(result.Success);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("Test", result.Data.Name);
        }
    }

    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
