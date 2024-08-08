using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub_Tester.Helpers;
using Microsoft.Extensions.Options;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    [Collection("Sequential")]
    public class AppDataScannerTests : IAsyncLifetime
    {
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly Mock<ILogger<AppDataScanner>> _mockLogger;
        private readonly Mock<IAppStructureCache> _mockCache;
        private readonly FileStructureGenerator _fileStructureGenerator =
            new FileStructureGenerator();
        private AppDataScanner _appDataScanner;

        private string appsPath,
            internalPath,
            externalPath,
            appJson,
            token = "userToken";

        public AppDataScannerTests()
        {
            _mockFavoriteService = new Mock<IFavoriteService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _mockLogger = new Mock<ILogger<AppDataScanner>>();
            _mockCache = new Mock<IAppStructureCache>();
        }

        public Task InitializeAsync()
        {
            (appsPath, appJson, internalPath, externalPath) =
                _fileStructureGenerator.GenerateFileStructure();
            _appDataScanner = new AppDataScanner(
                _mockFavoriteService.Object,
                _mockPermissionService.Object,
                _mockLogger.Object,
                Options.Create(new AppSettings() { MyAppSettings = GetAppSettings() }),
                _mockCache.Object
            );
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await Task.Delay(100); // Small delay to allow file handles to be released
            _fileStructureGenerator.Dispose();
        }

        [Fact]
        public async Task GetAppsStructure_CheckWrongAppsPath()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );
            var settings = GetAppSettings();
            settings.AppsPath = "abc";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _appDataScanner.GetAppsStructure(token)
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckWrongInternalPath()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );
            var settings = GetAppSettings();
            settings.InternalPackagesPath = "abc";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _appDataScanner.GetAppsStructure(token)
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckWrongExternalPath()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );
            var settings = GetAppSettings();
            settings.ExternalPackagesPath = "abc";

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _appDataScanner.GetAppsStructure(token)
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckNoPermissions()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(new List<Permission>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _appDataScanner.GetAppsStructure(token)
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckStructure()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );

            // Act
            var structure = await _appDataScanner.GetAppsStructure(token);

            // Assert
            Assert.NotNull(structure);
            Assert.Equal(3, structure.Count);
            Assert.Equal(4, structure.FirstOrDefault(app => app.ID == "app1")?.Versions.Count);
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app1")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                    ?.Modules.Count
            );
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app1")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.2")
                    ?.Modules.Count
            );
            Assert.Single(
                structure
                    .FirstOrDefault(app => app.ID == "app1")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.3")
                    ?.Modules
            );
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app1")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.4-prefix")
                    ?.Modules.Count
            );

            Assert.Equal(2, structure[1].Versions.Count);
            Assert.Equal(2, structure.FirstOrDefault(app => app.ID == "app2")?.Versions.Count);
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app2")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                    ?.Modules.Count
            );
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app2")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.2")
                    ?.Modules.Count
            );

            Assert.Single(structure[2].Versions);
            Assert.Single(structure.FirstOrDefault(app => app.ID == "app3")?.Versions);
            Assert.Equal(
                2,
                structure
                    .FirstOrDefault(app => app.ID == "app3")
                    ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                    ?.Modules.Count
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckPublished()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );

            // Act
            var structure = await _appDataScanner.GetAppsStructure(token);

            // Assert
            Assert.NotNull(structure);
            foreach (var app in structure)
            {
                foreach (var ver in app.Versions)
                {
                    foreach (var module in ver.Modules)
                    {
                        if (
                            module.Name == "module2"
                            && (ver.ID == "0.1" || ver.ID == "0.2" || ver.ID == "0.4-prefix")
                        )
                            Assert.True(module.IsPublished);
                        else if (module.Name == "module3" && ver.ID == "0.2")
                            Assert.True(module.IsPublished);
                        else
                            Assert.False(module.IsPublished);
                    }
                }
            }
        }

        [Fact]
        public async Task GetAppsStructure_CheckPermissions()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );

            // Act
            var structure = await _appDataScanner.GetAppsStructure(token);

            // Assert
            Assert.NotNull(structure);
            Assert.Contains(structure, app => app.ID == "app1");
            Assert.Contains(structure, app => app.ID == "app3");
            Assert.DoesNotContain(structure, app => app.ID == "app2");
        }

        [Fact]
        public async Task GetAppsStructure_CheckFavorites()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    new List<Permission>
                    {
                        new Permission { Username = token, AppID = "app1" },
                        new Permission { Username = token, AppID = "app2" },
                        new Permission { Username = token, AppID = "app3" }
                    }
                );

            _mockFavoriteService
                .Setup(service => service.GetFavoriteList(token))
                .ReturnsAsync(
                    new List<Favorite>
                    {
                        new Favorite { Username = token, AppID = "app2" },
                        new Favorite { Username = token, AppID = "app3" }
                    }
                );

            // Act
            var structure = await _appDataScanner.GetAppsStructure(token);

            // Assert
            Assert.NotNull(structure);
            Assert.True(structure.FirstOrDefault(app => app.ID == "app2")?.IsFavourite);
            Assert.True(structure.FirstOrDefault(app => app.ID == "app3")?.IsFavourite);
            Assert.False(structure.FirstOrDefault(app => app.ID == "app1")?.IsFavourite);
        }

        private MyAppSettings GetAppSettings()
        {
            var appSettings = new MyAppSettings()
            {
                ApplicationConfigFile = appJson,
                AppsPath = appsPath,
                InternalPackagesPath = internalPath,
                ExternalPackagesPath = externalPath
            };
            return appSettings;
        }
    }
}
