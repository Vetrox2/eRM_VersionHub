using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub_Tester.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace eRM_VersionHub_Tester.Tests
{
    public class ChangeTagIntegrationTests
        : IClassFixture<WebApplicationFactory<Program>>,
            IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly TagService _tagService;
        private readonly AppDataScanner _appDataScanner;
        private string appsPath,
            appJson,
            internalPath,
            externalPath,
            userToken = "testUser";
        private readonly Mock<IFavoriteService> _mockFavoriteService;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly FileStructureGenerator _fileStructureGenerator =
            new FileStructureGenerator();

        public ChangeTagIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _tagService = new TagService();
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
            var versionID = "1.0";
            var newTag = "preview";
            var url = $"Tag?appID={appID}&versionID={versionID}&newTag={newTag}";

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

            var requestBody = new
            {
                appID,
                versionID,
                newTag
            };
            var content = requestBody.Serialize();

            // Act
            var response = await _client.PostAsync(url, null);

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

        private bool CheckIfTagIsChangedCorrectly(
            List<AppStructureDto> appStructure,
            string appID,
            string versionID,
            string newTag
        )
        {
            var newVersionID = TagService.SwapVersionTags(versionID, newTag);
            var app = appStructure.FirstOrDefault(app => app.ID == appID);
            if (app == null)
                return false;

            if (newVersionID != versionID && app.Versions.Any(v => v.ID == versionID))
                return false;

            return app.Versions.Any(v => v.ID == newVersionID);
        }
    }
}
