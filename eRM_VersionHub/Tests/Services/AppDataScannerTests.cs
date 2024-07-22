using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub_Tester.Helpers;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    [Collection("Sequential")]
    public class AppDataScannerTests : IAsyncLifetime
    {
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly AppDataScanner _appDataScanner;
        private readonly FileStructureGenerator _fileStructureGenerator = new FileStructureGenerator();

        private string appsPath, internalPath, externalPath, appJson, token = "userToken";

        public AppDataScannerTests()
        {
            _mockFavoriteService = new Mock<IFavoriteService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _appDataScanner = new AppDataScanner(
                _mockFavoriteService.Object,
                _mockPermissionService.Object
            );
        }

        public Task InitializeAsync()
        {
            (appsPath, appJson, internalPath, externalPath) =
                _fileStructureGenerator.GenerateFileStructure();
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
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );
            var settings = GetAppSettings();
            settings.AppsPath = "abc";

            // Act
            var structure = await _appDataScanner.GetAppsStructure(settings, token);

            // Assert
            Assert.False(structure.Success);
            Assert.True(structure.Errors?.Contains("Fatal error"));
        }

        [Fact]
        public async Task GetAppsStructure_CheckWrongInternalPath()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );
            var settings = GetAppSettings();
            settings.InternalPackagesPath = "abc";

            // Act
            var structure = await _appDataScanner.GetAppsStructure(settings, token);

            // Assert
            Assert.False(structure.Success);
            Assert.True(structure.Errors?.Contains("Fatal error"));
        }

        [Fact]
        public async Task GetAppsStructure_CheckWrongExternalPath()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );
            var settings = GetAppSettings();
            settings.ExternalPackagesPath = "abc";

            // Act
            var structure = await _appDataScanner.GetAppsStructure(settings, token);

            // Assert
            Assert.False(structure.Success);
            Assert.True(structure.Errors?.Contains("Fatal error"));
        }

        [Fact]
        public async Task GetAppsStructure_CheckNoPermissions()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse([]));

            // Act
            var structure = await _appDataScanner.GetAppsStructure(GetAppSettings(), token);

            // Assert
            Assert.False(structure.Success);
            Assert.True(structure.Errors?.Contains("User permissions not found"));
        }

        [Fact]
        public async Task GetAppsStructure_CheckStructure()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );

            // Act
            var response = await _appDataScanner.GetAppsStructure(GetAppSettings(),token);
            var structure = response.Data;

            // Assert
            Assert.NotNull(structure);

            if (structure != null)
            {
                Assert.True(structure.Count == 3);
                Assert.True(structure.FirstOrDefault(app => app.ID == "app1")?.Versions.Count == 4);
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app1")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                        ?.Modules.Count == 2
                );
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app1")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.2")
                        ?.Modules.Count == 2
                );
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app1")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.3")
                        ?.Modules.Count == 1
                );
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app1")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.4-prefix")
                        ?.Modules.Count == 2
                );

                Assert.True(structure[1].Versions.Count == 2);
                Assert.True(structure.FirstOrDefault(app => app.ID == "app2")?.Versions.Count == 2);
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app2")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                        ?.Modules.Count == 2
                );
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app2")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.2")
                        ?.Modules.Count == 2
                );

                Assert.True(structure[2].Versions.Count == 1);
                Assert.True(structure.FirstOrDefault(app => app.ID == "app3")?.Versions.Count == 1);
                Assert.True(
                    structure
                        .FirstOrDefault(app => app.ID == "app3")
                        ?.Versions.FirstOrDefault(version => version.ID == "0.1")
                        ?.Modules.Count == 2
                );
            }
        }

        [Fact]
        public async Task GetAppsStructure_CheckPublished()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );

            // Act
            var response = await _appDataScanner.GetAppsStructure(GetAppSettings(), token);
            var structure = response.Data;

            // Assert
            Assert.NotNull(structure);
            structure?.ForEach(app =>
                app.Versions.ForEach(ver =>
                    ver.Modules.ForEach(module =>
                    {
                        if (module.Name == "module2" && (ver.ID == "0.1" || ver.ID == "0.2" || ver.ID == "0.4-prefix"))
                            Assert.True(module.IsPublished);
                        else if (module.Name == "module3" && ver.ID == "0.2")
                            Assert.True(module.IsPublished);
                        else
                            Assert.False(module.IsPublished);
                    })
                )
            );
        }

        [Fact]
        public async Task GetAppsStructure_CheckPermissions()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );

            // Act
            var response = await _appDataScanner.GetAppsStructure(GetAppSettings(), token);
            var structure = response.Data;

            // Assert
            Assert.NotNull(structure);
            Assert.True(structure?.Any(app => app.ID == "app1"));
            Assert.True(structure?.Any(app => app.ID == "app3"));
            Assert.False(structure?.Any(app => app.ID == "app2"));
        }

        [Fact]
        public async Task GetAppsStructure_CheckFavorites()
        {
            // Arrange
            _mockPermissionService
                .Setup(service => service.GetPermissionList(token))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = token, AppID = "app1" },
                            new Permission() { Username = token, AppID = "app2" },
                            new Permission() { Username = token, AppID = "app3" }
                        ]
                    )
                );

            _mockFavoriteService
                .Setup(service => service.GetFavoriteList(token))
                .ReturnsAsync(
                    ApiResponse<List<Favorite>>.SuccessResponse(
                        [
                            new Favorite() { Username = token, AppID = "app2" },
                            new Favorite() { Username = token, AppID = "app3" }
                        ]
                    )
                );

            // Act
            var response = await _appDataScanner.GetAppsStructure(GetAppSettings(), token);
            var structure = response.Data;

            // Assert
            Assert.NotNull(structure);
            Assert.True(structure?.FirstOrDefault(app => app.ID == "app2")?.IsFavourite);
            Assert.True(structure?.FirstOrDefault(app => app.ID == "app3")?.IsFavourite);
            Assert.False(structure?.FirstOrDefault(app => app.ID == "app1")?.IsFavourite);
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
