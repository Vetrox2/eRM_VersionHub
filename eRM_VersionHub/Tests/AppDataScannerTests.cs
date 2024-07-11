using Moq;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub.Models;

namespace eRM_VersionHub_Tester
{
    public class AppDataScannerTests
    {
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly AppDataScanner _appDataScanner;

        private readonly string common = "./Disc/", packages = "packages", appJson = "application.json", token = "user";
        private readonly string appsPath, internalPath, externalPath;

        public AppDataScannerTests()
        {
            _mockFavoriteService = new Mock<IFavoriteService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _appDataScanner = new AppDataScanner(_mockFavoriteService.Object, _mockPermissionService.Object);

            appsPath = Path.Combine(common, "apps");
            internalPath = Path.Combine(common, "eRMwewn", packages);
            externalPath = Path.Combine(common, "eRMzewn", packages);
        }

        [Fact]
        public async Task GetAppsStructure_CheckNoPublished()
        {
            _mockPermissionService.Setup(service => service.GetPermissionList(token))
            .ReturnsAsync(ApiResponse<List<Permission>>.SuccessResponse(
            [
                new Permission() { Username = token, AppID = "adminxe" },
                new Permission() { Username = token, AppID = "akcyza2" },
                new Permission() { Username = token, AppID = "alerter2" }
            ]));

            bool result = true;

            var structure = await _appDataScanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, token);
            Assert.NotNull(structure);
            structure.ForEach(app => app.Versions.ForEach(ver => ver.Modules.ForEach(module =>
            {
                if (module.IsPublished)
                {
                    result = false;
                }
            }
            )));
            Assert.True(result);
        }

        [Fact]
        public async Task GetAppsStructure_CheckSomePublished()
        {
            _mockPermissionService.Setup(service => service.GetPermissionList(token))
            .ReturnsAsync(ApiResponse<List<Permission>>.SuccessResponse(
            [
                new Permission() { Username = token, AppID = "1" },
                new Permission() { Username = token, AppID = "2" },
                new Permission() { Username = token, AppID = "3" }
            ]));

            bool result = true;
            var structure = await _appDataScanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, token);
            Assert.NotNull(structure);
            structure.ForEach(app => app.Versions.ForEach(ver => ver.Modules.ForEach(module =>
            {
                if (module.IsPublished && module.Name != "test")
                    result = false;
                if (module.IsPublished && module.Name == "test" && !(ver.ID == "1" || ver.ID == "2"))
                    result = false;
            }
            )));
            Assert.True(result);
        }
    }
}