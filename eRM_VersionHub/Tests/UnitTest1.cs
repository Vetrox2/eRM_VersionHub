using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using Newtonsoft.Json.Linq;
using eRM_VersionHub.Result;
using System.Security;

public class AppDataScannerTests
{
    private readonly Mock<IFavoriteService> _mockFavoriteService;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly AppDataScanner _appDataScanner;

    public AppDataScannerTests()
    {
        _mockFavoriteService = new Mock<IFavoriteService>();
        _mockPermissionService = new Mock<IPermissionService>();
        _appDataScanner = new AppDataScanner(_mockFavoriteService.Object, _mockPermissionService.Object);
    }

    [Fact]
    public async Task GetAppsStructure_CheckNoPublished()
    {
        var token = "user";
        _mockPermissionService.Setup(service => service.GetPermissionList(token))
        .ReturnsAsync(Result<List<Permission>>.Success(new List<Permission>()
        {
                new Permission() { Username = token, AppID = "1" },
                new Permission() { Username = token, AppID = "2" },
                new Permission() { Username = token, AppID = "3" }
            }));

        bool result = true;
        var structure = await _appDataScanner.GetAppsStructure("Disc\\apps", "application.json", "Disc\\Disc1\\eRMwewn", "Disc\\Disc1\\eRMzewn", token);
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
        var token = "user";
        _mockPermissionService.Setup(service => service.GetPermissionList(token))
        .ReturnsAsync(Result<List<Permission>>.Success(new List<Permission>()
        {
                new Permission() { Username = token, AppID = "1" },
                new Permission() { Username = token, AppID = "2" },
                new Permission() { Username = token, AppID = "3" }
            }));

        bool result = true;
        var structure = await _appDataScanner.GetAppsStructure("Disc\\apps", "application.json", "Disc\\Disc2\\eRMwewn", "Disc\\Disc2\\eRMzewn", token);
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

    //[Fact]
    //public async Task GetAppsStructure_ReturnsNull_WhenNoPermissions()
    //{
    //    // Arrange
    //    var token = "user";
    //    _mockPermissionService.Setup(service => service.GetPermissionList(token))
    //        .ReturnsAsync(Result<List<Permission>>.Success(new List<Permission>() 
    //        { 
    //            new Permission() { Username = token, AppID = "alerter2" },
    //            new Permission() { Username = token, AppID = "rgml" },
    //            new Permission() { Username = token, AppID = "zin" }
    //        }));

    //    // Act
    //    var result = await _appDataScanner.GetAppsStructure("appsPath", "appJsonName", "internalPackagesPath", "externalPackagesPath", token);

    //    // Assert
    //    Assert.Null(result);
    //}

    //[Fact]
    //public async Task GetAppsStructure_FiltersByPermissions()
    //{
    //    // Arrange
    //    var token = "userToken";
    //    var perms = new List<PermissionDto> { new PermissionDto { AppID = "app1" } };
    //    var favs = new List<FavoriteDto> { new FavoriteDto { AppID = "app1" } };
    //    _mockPermissionService.Setup(service => service.GetPermissionList(token))
    //        .ReturnsAsync(new ServiceResponse<List<PermissionDto>> { IsSuccess = true, Data = perms });
    //    _mockFavoriteService.Setup(service => service.GetFavoriteList(token))
    //        .ReturnsAsync(new ServiceResponse<List<FavoriteDto>> { IsSuccess = true, Data = favs });

    //    var appStructure = new List<AppStructureDto>
    //    {
    //        new AppStructureDto { ID = "app1", Versions = new List<VersionDto>(), IsFavourite = false },
    //        new AppStructureDto { ID = "app2", Versions = new List<VersionDto>(), IsFavourite = false }
    //    };

    //    // Act
    //    var result = await _appDataScanner.GetFilteredByPermsAndFav(appStructure, token);

    //    // Assert
    //    Assert.Single(result);
    //    Assert.Equal("app1", result[0].ID);
    //    Assert.True(result[0].IsFavourite);
    //}
}
