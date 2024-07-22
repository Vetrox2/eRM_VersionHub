using eRM_VersionHub.Services.Interfaces;
using eRM_VersionHub.Services;
using eRM_VersionHub_Tester.Helpers;
using Moq;
using eRM_VersionHub.Models;
using eRM_VersionHub.Dtos;
using System.Text;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class PublicationControllerTests : IAsyncLifetime
    {
        private TestFixture _factory;
        private HttpClient _client;
        private string appsPath, appJson, internalPath, externalPath, userToken = "testUser";
        private readonly FileStructureGenerator _fileStructureGenerator = new FileStructureGenerator();

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
        public async Task Publish_ShouldReturnSuccess()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.1",
                    [
                        new ModuleDto() { Name = "module4" },
                        new ModuleDto() { Name = "module5" }
                    ])
                ];

            // Act
            var response = await _client.PostAsync(GetUrl(), JsonContent.Create(versions));

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task Publish_VersionDoesntExist()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.5",
                    [
                        new ModuleDto() { Name = "module4" },
                        new ModuleDto() { Name = "module5" }
                    ])
                ];

            // Act
            var response = await _client.PostAsync(GetUrl(), JsonContent.Create(versions));

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        [Fact]
        public async Task Publish_VersionIsAlreadyFullyPublished()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.2",
                    [
                        new ModuleDto() { Name = "module2" },
                        new ModuleDto() { Name = "module3" }
                    ])
                ];

            // Act
            var response = await _client.PostAsync(GetUrl(), JsonContent.Create(versions));

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        [Fact]
        public async Task Publish_VersionIsAlreadyPartiallyPublished()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.1",
                    [
                        new ModuleDto() { Name = "module2" },
                        new ModuleDto() { Name = "module3" }
                    ])
                ];

            // Act
            var response = await _client.PostAsync(GetUrl(), JsonContent.Create(versions));

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        [Fact]
        public async Task Publish_SomeModulesDontExist()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.2",
                    [
                        new ModuleDto() { Name = "module4" },
                        new ModuleDto() { Name = "module5" }
                    ])
                ];

            // Act
            var response = await _client.PostAsync(GetUrl(), JsonContent.Create(versions));

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        [Fact]
        public async Task UnPublish_Success()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.2",
                    [
                        new ModuleDto() { Name = "module2" },
                        new ModuleDto() { Name = "module3" }
                    ])
                ];

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(versions),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(GetUrl(), UriKind.Relative)
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task UnPublish_VersionIsNotPublished()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.1",
                    [
                        new ModuleDto() { Name = "module4" },
                        new ModuleDto() { Name = "module5" }
                    ])
                ];

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(versions),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(GetUrl(), UriKind.Relative)
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        [Fact]
        public async Task UnPublish_SomeModulesAreNotPublished()
        {
            // Arrange
            List<VersionDto> versions =
                [
                new VersionDto("0.1",
                    [
                        new ModuleDto() { Name = "module2" },
                        new ModuleDto() { Name = "module3" }
                    ])
                ];

            HttpRequestMessage request = new HttpRequestMessage
            {
                Content = JsonContent.Create(versions),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(GetUrl(), UriKind.Relative)
            };

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = responseContent.Deserialize<ApiResponse<string>>();

            Assert.False(apiResponse.Success);
        }

        private string GetUrl() => "/Publication";

    }
}
