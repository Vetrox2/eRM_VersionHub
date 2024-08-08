using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Database;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<IPermissionRepository> _mockRepository;
        private readonly Mock<ILogger<PermissionService>> _mockLogger;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly PermissionService _permissionService;

        public PermissionServiceTests()
        {
            _mockRepository = new Mock<IPermissionRepository>();
            _mockLogger = new Mock<ILogger<PermissionService>>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _permissionService = new PermissionService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockServiceProvider.Object
            );
        }

        [Fact]
        public async Task CreatePermission_ShouldCallRepositoryMethod()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockRepository
                .Setup(repo => repo.CreatePermission(It.IsAny<Permission>()))
                .ReturnsAsync(permission);

            // Act
            var result = await _permissionService.CreatePermission(permission);

            // Assert
            Assert.Equal(permission, result);
            _mockRepository.Verify(repo => repo.CreatePermission(permission), Times.Once);
        }

        [Fact]
        public async Task GetPermissionList_ShouldCallRepositoryMethod()
        {
            // Arrange
            var username = "testUser";
            var permissions = new List<Permission>
            {
                new Permission { Username = username, AppID = "testApp" }
            };
            _mockRepository
                .Setup(repo => repo.GetPermissionList(username))
                .ReturnsAsync(permissions);

            // Act
            var result = await _permissionService.GetPermissionList(username);

            // Assert
            Assert.Equal(permissions, result);
            _mockRepository.Verify(repo => repo.GetPermissionList(username), Times.Once);
        }

        [Fact]
        public async Task DeletePermission_ShouldCallRepositoryMethod()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockRepository
                .Setup(repo => repo.DeletePermission(It.IsAny<Permission>()))
                .ReturnsAsync(permission);

            // Act
            var result = await _permissionService.DeletePermission(permission);

            // Assert
            Assert.Equal(permission, result);
            _mockRepository.Verify(repo => repo.DeletePermission(permission), Times.Once);
        }

        /*        [Fact]
                public async Task GetAllPermissionList_ShouldReturnAppPermissionDto()
                {
                    // Arrange
                    var username = "testUser";
                    var user = new User { Username = username };
                    var permissions = new List<Permission>
                    {
                        new Permission { Username = username, AppID = "app1" },
                        new Permission { Username = username, AppID = "app2" }
                    };
                    var apps = new List<string> { "app1", "app2", "app3" };

                    _mockUserRepository.Setup(repo => repo.GetUser(username)).ReturnsAsync(user);
                    _mockRepository
                        .Setup(repo => repo.GetPermissionList(username))
                        .ReturnsAsync(permissions);

                    var mockAppDataScanner = new Mock<IAppDataScanner>();
                    mockAppDataScanner.Setup(scanner => scanner.GetAppsNames()).Returns(apps);
                    _mockServiceProvider
                        .Setup(sp => sp.GetRequiredService<IAppDataScanner>())
                        .Returns(mockAppDataScanner.Object);

                    // Act
                    var result = await _permissionService.GetAllPermissionList(username);

                    // Assert
                    Assert.NotNull(result);
                    Assert.Equal(username, result.User);
                    Assert.Equal(3, result.AppsPermission.Count);
                    Assert.True(result.AppsPermission["app1"]);
                    Assert.True(result.AppsPermission["app2"]);
                    Assert.False(result.AppsPermission["app3"]);
                }

                [Fact]
                public async Task GetAllPermissionList_ShouldThrowExceptionWhenUserNotFound()
                {
                    // Arrange
                    var username = "nonexistentUser";
                    _mockUserRepository.Setup(repo => repo.GetUser(username)).ReturnsAsync((User)null);

                    // Act & Assert
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        () => _permissionService.GetAllPermissionList(username)
                    );
                }

                [Fact]
                public async Task ValidatePermissions_ShouldReturnTrueWhenAllModulesArePermitted()
                {
                    // Arrange
                    var username = "testUser";
                    var version = new VersionDto
                    {
                        Modules = new List<ModuleDto>
                        {
                            new ModuleDto { Name = "module1" },
                            new ModuleDto { Name = "module2" }
                        }
                    };
                    var settings = new MyAppSettings
                    {
                        AppsPath = "/apps",
                        ApplicationConfigFile = "config.json"
                    };
                    var permissions = new List<Permission>
                    {
                        new Permission { Username = username, AppID = "app1" },
                        new Permission { Username = username, AppID = "app2" }
                    };

                    _mockRepository
                        .Setup(repo => repo.GetPermissionList(username))
                        .ReturnsAsync(permissions);

                    // Mock AppDataScanner.GetAppJsonModel to return app models with the required modules
                    AppDataScanner.GetAppJsonModel = (string path) =>
                        new AppModel
                        {
                            Modules = new List<ModuleModel>
                            {
                                new ModuleModel { ModuleId = "module1" },
                                new ModuleModel { ModuleId = "module2" }
                            }
                        };

                    // Act
                    var result = await _permissionService.ValidatePermissions(version, settings, username);

                    // Assert
                    Assert.True(result);
                }

                [Fact]
                public async Task ValidatePermissions_ShouldReturnFalseWhenModuleIsNotPermitted()
                {
                    // Arrange
                    var username = "testUser";
                    var version = new VersionDto
                    {
                        Modules = new List<ModuleDto>
                        {
                            new ModuleDto { Name = "module1" },
                            new ModuleDto { Name = "module3" } // This module is not in the permitted list
                        }
                    };
                    var settings = new MyAppSettings
                    {
                        AppsPath = "/apps",
                        ApplicationConfigFile = "config.json"
                    };
                    var permissions = new List<Permission>
                    {
                        new Permission { Username = username, AppID = "app1" },
                        new Permission { Username = username, AppID = "app2" }
                    };

                    _mockRepository
                        .Setup(repo => repo.GetPermissionList(username))
                        .ReturnsAsync(permissions);

                    // Mock AppDataScanner.GetAppJsonModel to return app models with only some of the required modules
                    AppDataScanner.GetAppJsonModel = (string path) =>
                        new AppModel
                        {
                            Modules = new List<ModuleModel>
                            {
                                new ModuleModel { ModuleId = "module1" },
                                new ModuleModel { ModuleId = "module2" }
                            }
                        };

                    // Act
                    var result = await _permissionService.ValidatePermissions(version, settings, username);

                    // Assert
                    Assert.False(result);
                }*/
    }
}
