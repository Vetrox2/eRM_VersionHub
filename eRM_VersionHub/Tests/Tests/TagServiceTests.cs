using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub_Tester.Services;

namespace eRM_VersionHub_Tester.Tests
{
    public class TagServiceTests : IAsyncLifetime
    {
        private readonly TagService _tagService;
        private readonly FileStructureGenerator _fileStructureGenerator =
            new FileStructureGenerator();
        private string appsPath,
            appJson,
            internalPath,
            externalPath;

        public TagServiceTests()
        {
            _tagService = new TagService();
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
        public void SetTag_ShouldReturnSuccessResponse()
        {
            // Arrange
            var settings = new MyAppSettings()
            {
                ApplicationConfigFile = appJson,
                AppsPath = appsPath,
                ExternalPackagesPath = externalPath,
                InternalPackagesPath = internalPath
            };
            var appID = "app2";
            var versionID = "0.1";
            var newTag = "testTag";

            // Act
            var response = _tagService.SetTag(settings, appID, versionID, newTag);

            // Assert
            Assert.True(response.Success);
            Assert.Equal(
                "Internal modules version modified: 2\nPublished modules version modified: 1",
                response.Data
            );
        }

        [Fact]
        public void SetTag_ShouldReturnAppNotFound()
        {
            // Arrange
            var settings = new MyAppSettings()
            {
                ApplicationConfigFile = appJson,
                AppsPath = appsPath,
                ExternalPackagesPath = externalPath,
                InternalPackagesPath = internalPath
            };
            var appID = "app4";
            var versionID = "0.1";
            var newTag = "testTag";

            // Act
            var response = _tagService.SetTag(settings, appID, versionID, newTag);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("App not found", response.Errors[0]);
        }

        [Fact]
        public void SetTag_ShouldReturnNoneVersionModified()
        {
            // Arrange
            var settings = new MyAppSettings()
            {
                ApplicationConfigFile = appJson,
                AppsPath = appsPath,
                ExternalPackagesPath = externalPath,
                InternalPackagesPath = internalPath
            };
            var appID = "app2";
            var versionID = "0.4";
            var newTag = "testTag";

            // Act
            var response = _tagService.SetTag(settings, appID, versionID, newTag);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Version for none module was modified", response.Errors[0]);
        }
    }
}
