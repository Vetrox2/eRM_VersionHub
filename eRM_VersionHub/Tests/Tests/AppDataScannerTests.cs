using Moq;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub.Models;
using eRM_VersionHub_Tester.Services;

namespace eRM_VersionHub_Tester.Tests
{
    public class AppDataScannerTests : IAsyncLifetime

    {
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly AppDataScanner _appDataScanner;

        private string appsPath, internalPath, externalPath, appJson, token = "userToken";

        public AppDataScannerTests()
        {
            _mockFavoriteService = new Mock<IFavoriteService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _appDataScanner = new AppDataScanner(_mockFavoriteService.Object, _mockPermissionService.Object);
        }
        public Task InitializeAsync()
        {
            (appsPath, appJson, internalPath, externalPath) = FileStructureGenerator.GenerateFileStructure();
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            FileStructureGenerator.DeleteFileStructure();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAppsStructure_CheckNoPublished()
        {
            // Arrange
            _mockPermissionService.Setup(service => service.GetPermissionList(token))
            .ReturnsAsync(ApiResponse<List<Permission>>.SuccessResponse(
            [
                new Permission() { Username = token, AppID = "app1" },
                new Permission() { Username = token, AppID = "app2" },
                new Permission() { Username = token, AppID = "app3" }
            ]));

            bool result = true;

            // Act
            var structure = await _appDataScanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, token);

            // Assert
            Assert.NotNull(structure);
            structure?.ForEach(app => app.Versions.ForEach(ver => ver.Modules.ForEach(module =>
            {
                if (module.IsPublished)
                {
                    result = false;
                }
            }
            )));
            Assert.True(result);
            if (structure != null)
            {
                Assert.True(structure.Count == 3);
                Assert.True(structure[0].Versions.Count == 3);
                Assert.True(structure[0].Versions[2].Modules.Count == 1);
                Assert.True(structure[1].Versions.Count == 2);
                Assert.True(structure[2].Versions.Count == 1);
            }

        }

        //[Fact]
        //public async Task GetAppsStructure_CheckSomePublished()
        //{
        //    _mockPermissionService.Setup(service => service.GetPermissionList(token))
        //    .ReturnsAsync(ApiResponse<List<Permission>>.SuccessResponse(
        //    [
        //        new Permission() { Username = token, AppID = "app1" },
        //        new Permission() { Username = token, AppID = "app2" },
        //        new Permission() { Username = token, AppID = "app3" }
        //    ]));

        //    bool result = true;
        //    var structure = await _appDataScanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, token);
        //    Assert.NotNull(structure);
        //    structure.ForEach(app => app.Versions.ForEach(ver => ver.Modules.ForEach(module =>
        //    {
        //        if (module.IsPublished && module.Name != "module1")
        //            result = false;
        //        if (module.IsPublished && module.Name == "test" && !(ver.ID == "1" || ver.ID == "2"))
        //            result = false;
        //    }
        //    )));
        //    Assert.True(result);
        //}


    }
}