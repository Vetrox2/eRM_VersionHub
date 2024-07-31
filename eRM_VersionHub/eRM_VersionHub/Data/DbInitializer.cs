using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using static eRM_VersionHub.Repositories.DbRepository;

namespace eRM_VersionHub.Data
{
    public class DbInitializer
    {
        private IDbRepository _dbRepository;
        private IFavoriteRepository _favoriteRepository;
        private IPermissionRepository _permissionRepository;
        private IUserRepository _userRepository;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(WebApplication app, ILogger<DbInitializer> logger)
        {
            _logger = logger;
            _logger.LogDebug(AppLogEvents.Database, "Initializing database");
            InitDb(app);
        }

        public void InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            _dbRepository = scope.ServiceProvider.GetService<IDbRepository>();
            _favoriteRepository = scope.ServiceProvider.GetService<IFavoriteRepository>();
            _permissionRepository = scope.ServiceProvider.GetService<IPermissionRepository>();
            _userRepository = scope.ServiceProvider.GetService<IUserRepository>();

            _logger.LogDebug(AppLogEvents.Database, "Getting needed services (DbRepository, FavoriteRepository, PermissionRepository, UserRepository)");
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

            _logger.LogDebug(AppLogEvents.Database, "Creating table permission with data: {columns}", columns);
            var result = await _dbRepository.CreateTable("permissions", columns);
            _logger.LogDebug(AppLogEvents.Database, "Result of CreatePermissionTable: {result}", result);

            if (!result.Success)
            {
                _logger.LogError(AppLogEvents.Database, "CreatePermissionTable returned: {Errors}", result.Errors);
                Console.WriteLine(
                    $"Failed to create Permissions table: {string.Join(", ", result.Errors)}"
                );
            }

            _logger.LogInformation(AppLogEvents.Database, "CreatePermissionTable returned: {result}", result.Data);
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

            _logger.LogDebug(AppLogEvents.Database, "Creating table favorites with data: {columns}", columns);
            var result = await _dbRepository.CreateTable("favorites", columns);
            _logger.LogDebug(AppLogEvents.Database, "Result of CreateFavoriteTable: {result}", result);

            if (!result.Success)
            {
                _logger.LogError(AppLogEvents.Database, "CreateFavoriteTable returned: {Errors}", result.Errors);
                Console.WriteLine(
                   $"Failed to create Favorites table: {string.Join(", ", result.Errors)}"
               );
            }

            _logger.LogInformation(AppLogEvents.Database, "CreateFavoriteTable returned: {result}", result.Data);
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

            _logger.LogDebug(AppLogEvents.Database, "Creating table users with data: {columns}", columns);
            var result = await _dbRepository.CreateTable("users", columns);
            _logger.LogDebug(AppLogEvents.Database, "Result of CreateUserTable: {result}", result);

            if (!result.Success)
            {
                _logger.LogError(AppLogEvents.Database, "CreateUserTable returned: {Errors}", result.Errors);
                Console.WriteLine(
                    $"Failed to create Users table: {string.Join(", ", result.Errors)}"
                );
            }

            _logger.LogInformation(AppLogEvents.Database, "CreateUserTable returned: {result}", result.Data);
        }

        private async Task SeedData()
        {
            Permission permission = new() { Username = "admin", AppID = "adminxe" };
            _logger.LogDebug(AppLogEvents.Database, "Creating permission: {permission}", permission);

            ApiResponse<Permission?> resultP = await _permissionRepository.CreatePermission(permission);
            _logger.LogInformation(AppLogEvents.Database, "Result of CreatePermission: {Data}", resultP.Data);

            Favorite favorite = new() { Username = "admin", AppID = "app456" };
            _logger.LogDebug(AppLogEvents.Database, "Creating favorite: {favorite}", favorite);

            ApiResponse<Favorite?> resultF = await _favoriteRepository.CreateFavorite(favorite);
            _logger.LogInformation(AppLogEvents.Database, "Result of CreateFavorite: {Data}", resultF.Data);

            User user = new() { Username = "admin", CreationDate = DateTime.UtcNow };
            _logger.LogDebug(AppLogEvents.Database, "Creating user: {user}", user);

            ApiResponse<User?> resultU = await _userRepository.CreateUser(user);
            _logger.LogInformation(AppLogEvents.Database, "Result of CreateUser: {Data}", resultU.Data);
        }
    }
}