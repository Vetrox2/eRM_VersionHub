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

        public DbInitializer(WebApplication app)
        {
            InitDb(app);
        }

        public void InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            _dbRepository = scope.ServiceProvider.GetService<IDbRepository>();
            _favoriteRepository = scope.ServiceProvider.GetService<IFavoriteRepository>();
            _permissionRepository = scope.ServiceProvider.GetService<IPermissionRepository>();
            _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
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
                    IsNullable = false
                },
                new ColumnDefinition
                {
                    Name = "app_id",
                    Type = "VARCHAR(255)",
                    IsNullable = false
                }
            };

            var result = await _dbRepository.CreateTable("permissions", columns);
            if (!result.Success)
            {
                Console.WriteLine(
                    $"Failed to create Permissions table: {string.Join(", ", result.Errors)}"
                );
            }
        }

        private async Task CreateFavoriteTable()
        {
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "username",
                    Type = "VARCHAR(255)",
                    IsNullable = false
                },
                new ColumnDefinition
                {
                    Name = "app_id",
                    Type = "VARCHAR(255)",
                    IsNullable = false
                }
            };

            var result = await _dbRepository.CreateTable("favorites", columns);
            if (!result.Success)
            {
                Console.WriteLine(
                    $"Failed to create Favorites table: {string.Join(", ", result.Errors)}"
                );
            }
        }

        private async Task CreateUserTable()
        {
            var columns = new List<ColumnDefinition>
            {
                new ColumnDefinition
                {
                    Name = "username",
                    Type = "VARCHAR(255)",
                    IsNullable = false,
                    IsPrimaryKey = true
                },
                new ColumnDefinition
                {
                    Name = "creation_date",
                    Type = "TIMESTAMP",
                    IsNullable = false
                }
            };

            var result = await _dbRepository.CreateTable("users", columns);
            if (!result.Success)
            {
                Console.WriteLine(
                    $"Failed to create Users table: {string.Join(", ", result.Errors)}"
                );
            }
        }

        private async Task SeedData()
        {
            await _permissionRepository.CreatePermission(
                new Permission { Username = "admin", AppID = "adminxe" }
            );

            await _favoriteRepository.CreateFavorite(
                new Favorite { Username = "admin", AppID = "app456" }
            );

            await _userRepository.CreateUser(
                new User { Username = "admin", CreationDate = DateTime.UtcNow }
            );
        }
    }
}
