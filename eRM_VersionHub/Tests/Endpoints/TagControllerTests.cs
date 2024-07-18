using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub_Tester.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class TagControllerTests : IAsyncLifetime
    {
        private TestFixture _factory;
        private HttpClient _client;
        private readonly TagService _tagService;
        private readonly AppDataScanner _appDataScanner;
        private string appsPath, appJson, internalPath, externalPath, userToken = "testUser";
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly FileStructureGenerator _fileStructureGenerator = new FileStructureGenerator();

        public TagControllerTests()
        {
            _tagService = new TagService();
            _mockFavoriteService = new Mock<IFavoriteService>();
            _mockPermissionService = new Mock<IPermissionService>();
            _appDataScanner = new(_mockFavoriteService.Object, _mockPermissionService.Object);
        }

        public Task InitializeAsync()
        {
            (appsPath, appJson, internalPath, externalPath) = _fileStructureGenerator.GenerateFileStructure();

            _factory = new TestFixture();
            _factory.SetNewAppSettings(appJson, appsPath, internalPath, externalPath);
            _client = _factory.CreateClient();
            Thread.Sleep(100);
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _fileStructureGenerator.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task ChangeTag_ShouldUpdateTagCorrectly()
        {
            // Arrange
            var appID = "app1";
            var versionID = "0.1";
            var newTag = "preview";

            _mockPermissionService
                .Setup(service => service.GetPermissionList(userToken))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.SuccessResponse(
                        [
                            new Permission() { Username = userToken, AppID = "app1" },
                            new Permission() { Username = userToken, AppID = "app2" },
                            new Permission() { Username = userToken, AppID = "app3" }
                        ]
                    )
                );

            // Act
            var response = await _client.PostAsync(GetUrl(appID, versionID, newTag), null);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
            Assert.True(apiResponse.Success);

            var appStructure = await _appDataScanner.GetAppsStructure(
                appsPath,
                appJson,
                internalPath,
                externalPath,
                userToken
            );
            Assert.True(CheckIfTagIsChangedCorrectly(appStructure, appID, versionID, newTag));
        }

        [Fact]
        public async Task ChangeTag_ShouldRespondAppNotFound()
        {
            // Arrange
            var appID = "app4";
            var versionID = "0.1";
            var newTag = "preview";

            // Act
            var response = await _client.PostAsync(GetUrl(appID, versionID, newTag), null);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
            Assert.False(apiResponse.Success);
            Assert.Equal("App not found", apiResponse.Errors[0]);
        }

        [Fact]
        public async Task ChangeTag_ShouldRespondNoneVersionModified()
        {
            // Arrange
            var appID = "app2";
            var versionID = "1.1";
            var newTag = "preview";

            // Act
            var response = await _client.PostAsync(GetUrl(appID, versionID, newTag), null);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
            Assert.False(apiResponse.Success);
            Assert.Equal("Version for none module was modified", apiResponse.Errors[0]);
        }

        [Fact]
        public async Task ChangeTag_AppJsonFileIsCorrupted()
        {
            // Arrange
            var appID = "app2";
            var versionID = "0.1";
            var newTag = "preview";

            File.WriteAllText(Path.Combine(appsPath, "app2", appJson), "test123");

            // Act
            var response = await _client.PostAsync(GetUrl(appID, versionID, newTag), null);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
            Assert.False(apiResponse.Success);
            Assert.Equal("App not found", apiResponse.Errors[0]);
        }

        [Fact]
        public async Task ChangeTag_NewTagIsTheSame()
        {
            // Arrange
            var appID = "app2";
            var versionID = "0.1";
            var newTag = "";


            // Act
            var response = await _client.PostAsync(GetUrl(appID, versionID, newTag), null);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
            Assert.False(apiResponse.Success);
            Assert.Equal("New tag is the same as the old one", apiResponse.Errors[0]);
        }

        private bool CheckIfTagIsChangedCorrectly(
            List<AppStructureDto> appStructure,
            string appID,
            string versionID,
            string newTag
        )
        {
            var newVersionID = TagService.SwapVersionTag(versionID, newTag);
            var app = appStructure.FirstOrDefault(app => app.ID == appID);
            if (app == null)
                return false;

            if (newVersionID != versionID && app.Versions.Any(v => v.ID == versionID))
                return false;

            return app.Versions.Any(v => v.ID == newVersionID);
        }

        private string GetUrl(string appID, string versionID, string newTag) => $"/Tag?appID={appID}&versionID={versionID}&newTag={newTag}";
    }
}
