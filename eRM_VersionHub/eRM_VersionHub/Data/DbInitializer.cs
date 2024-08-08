using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using static eRM_VersionHub.Repositories.DbRepository;

namespace eRM_VersionHub.Data
{
    public class DbInitializer
    {
        private readonly IDbRepository _dbRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(WebApplication app, ILogger<DbInitializer> logger)
        {
            _logger = logger;
            _logger.LogDebug(AppLogEvents.Database, "Initializing database");
            using var scope = app.Services.CreateScope();
            _dbRepository = scope.ServiceProvider.GetRequiredService<IDbRepository>();
            _favoriteRepository = scope.ServiceProvider.GetRequiredService<IFavoriteRepository>();
            _permissionRepository =
                scope.ServiceProvider.GetRequiredService<IPermissionRepository>();
            _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            CreateTables();
        }

        private async Task CreateTables()
        {
            await CreatePermissionTable();
            await CreateFavoriteTable();
            await CreateUserTable();
            await SeedData();
        }

        private async Task CreatePermissionTable()
        {
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "username",
                    Type = "VARCHAR(255)",
                    NotNull = true
                },
                new ColumnDefinition
                {
                    Name = "app_id",
                    Type = "VARCHAR(255)",
                    NotNull = true
                }
            };

            _logger.LogDebug(
                AppLogEvents.Database,
                "Creating table permission with data: {columns}",
                columns
            );
            var result = await _dbRepository.CreateTable("permissions", columns);
            _logger.LogDebug(
                AppLogEvents.Database,
                "Result of CreatePermissionTable: {result}",
                result
            );

            if (!result)
            {
                throw new InvalidOperationException("Failed to create Permissions table");
            }

            _logger.LogInformation(
                AppLogEvents.Database,
                "CreatePermissionTable returned: {result}",
                result
            );
        }

        private async Task CreateFavoriteTable()
        {
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "username",
                    Type = "VARCHAR(255)",
                    NotNull = true
                },
                new ColumnDefinition
                {
                    Name = "app_id",
                    Type = "VARCHAR(255)",
                    NotNull = true
                }
            };

            _logger.LogDebug(
                AppLogEvents.Database,
                "Creating table favorites with data: {columns}",
                columns
            );
            var result = await _dbRepository.CreateTable("favorites", columns);
            _logger.LogDebug(
                AppLogEvents.Database,
                "Result of CreateFavoriteTable: {result}",
                result
            );

            if (!result)
            {
                throw new InvalidOperationException("Failed to create Favorites table");
            }

            _logger.LogInformation(
                AppLogEvents.Database,
                "CreateFavoriteTable returned: {result}",
                result
            );
        }

        private async Task CreateUserTable()
        {
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "username",
                    Type = "VARCHAR(255)",
                    PrimaryKey = true
                },
                new ColumnDefinition
                {
                    Name = "creation_date",
                    Type = "TIMESTAMP",
                    NotNull = true
                }
            };

            _logger.LogDebug(
                AppLogEvents.Database,
                "Creating table users with data: {columns}",
                columns
            );
            var result = await _dbRepository.CreateTable("users", columns);
            _logger.LogDebug(AppLogEvents.Database, "Result of CreateUserTable: {result}", result);

            if (!result)
            {
                throw new InvalidOperationException("Failed to create Users table");
            }

            _logger.LogInformation(
                AppLogEvents.Database,
                "CreateUserTable returned: {result}",
                result
            );
        }

        private async Task SeedData()
        {
            Permission permission = new() { Username = "admin", AppID = "adminxe" };
            _logger.LogDebug(
                AppLogEvents.Database,
                "Creating permission: {permission}",
                permission
            );

            var resultP = await _permissionRepository.CreatePermission(permission);
            _logger.LogInformation(
                AppLogEvents.Database,
                "Result of CreatePermission: {Data}",
                resultP
            );

            Favorite favorite = new() { Username = "admin", AppID = "app456" };
            _logger.LogDebug(AppLogEvents.Database, "Creating favorite: {favorite}", favorite);

            var resultF = await _favoriteRepository.CreateFavorite(favorite);
            _logger.LogInformation(
                AppLogEvents.Database,
                "Result of CreateFavorite: {Data}",
                resultF
            );

            User user = new() { Username = "admin", CreationDate = DateTime.UtcNow };
            _logger.LogDebug(AppLogEvents.Database, "Creating user: {user}", user);

            var resultU = await _userRepository.CreateUser(user);
            _logger.LogInformation(AppLogEvents.Database, "Result of CreateUser: {Data}", resultU);
        }
    }
}
