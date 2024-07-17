// using eRM_VersionHub.Dtos;
// using eRM_VersionHub.Models;
// using eRM_VersionHub.Services;
// using eRM_VersionHub.Services.Interfaces;
// using eRM_VersionHub_Tester.Helpers;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Moq;
// using Xunit;

// namespace eRM_VersionHub_Tester.Endpoints
// {
//     public class AppsControllerTests : IAsyncLifetime
//     {
//         private TestFixture _factory;
//         private HttpClient _client;
//         private readonly Mock<IAppDataScanner> _mockAppDataScanner;
//         private string appsPath, appJson, internalPath, externalPath;
//         private readonly MyAppSettings _settings;
//         private readonly FileStructureGenerator _fileStructureGenerator = new FileStructureGenerator();

//         public AppsControllerTests()
//         {
//             _mockAppDataScanner = new Mock<IAppDataScanner>();
//             _settings = new MyAppSettings
//             {
//                 AppsPath = "testAppsPath",
//                 ApplicationConfigFile = "testAppConfig.json",
//                 InternalPackagesPath = "testInternalPath",
//                 ExternalPackagesPath = "testExternalPath"
//             };
//         }

//         public Task InitializeAsync()
//         {
//             (appsPath, appJson, internalPath, externalPath) = _fileStructureGenerator.GenerateFileStructure();

//             TestFixture.SetNewAppSettings(appJson, appsPath, internalPath, externalPath);
//             _factory = new TestFixture();
//             _client = _factory.CreateClient();
//             Thread.Sleep(100);
//             return Task.CompletedTask;
//         }

//         public Task DisposeAsync()
//         {
//             _fileStructureGenerator.Dispose();
//             return Task.CompletedTask;
//         }

//         [Fact]
//         public async Task GetStructure_ShouldReturnAppStructure()
//         {
//             // Arrange
//             var userName = "testUser";
//             var appStructure = new List<AppStructureDto>
//             {
//                 new AppStructureDto { ID = "app1", Versions = new List<VersionDto> { new VersionDto("0.1", new List<ModuleDto>()) } },
//                 new AppStructureDto { ID = "app2", Versions = new List<VersionDto> { new VersionDto("0.2", new List<ModuleDto>()) } }
//             };

//             _mockAppDataScanner
//                 .Setup(scanner => scanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, userName))
//                 .ReturnsAsync(appStructure);

//             // Act
//             var response = await _client.GetAsync($"/Apps/{userName}");

//             // Assert
//             response.EnsureSuccessStatusCode();
//             var responseContent = await response.Content.ReadAsStringAsync();
//             var apiResponse = responseContent.Deserialize<ApiResponse<List<AppStructureDto>>>();
//             Assert.True(apiResponse.Success);
//             Assert.Equal(appStructure.Count, apiResponse.Data.Count);
//         }

//         [Fact]
//         public async Task GetStructure_ShouldReturnNotFound_WhenStructureIsEmpty()
//         {
//             // Arrange
//             var userName = "testUser";
//             _mockAppDataScanner
//                 .Setup(scanner => scanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, userName))
//                 .ReturnsAsync(new List<AppStructureDto>());

//             // Act
//             var response = await _client.GetAsync($"/Apps/{userName}");

//             // Assert
//             Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
//             var responseContent = await response.Content.ReadAsStringAsync();
//             var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
//             Assert.False(apiResponse.Success);
//             Assert.Equal("Some error", apiResponse.Errors[0]);
//         }

//         [Fact]
//         public async Task GetStructure_ShouldReturnNotFound_WhenStructureIsNull()
//         {
//             // Arrange
//             var userName = "testUser";
//             _mockAppDataScanner
//                 .Setup(scanner => scanner.GetAppsStructure(appsPath, appJson, internalPath, externalPath, userName))
//                 .ReturnsAsync((List<AppStructureDto>)null);

//             // Act
//             var response = await _client.GetAsync($"/Apps/{userName}");

//             // Assert
//             Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
//             var responseContent = await response.Content.ReadAsStringAsync();
//             var apiResponse = responseContent.Deserialize<ApiResponse<string>>();
//             Assert.False(apiResponse.Success);
//             Assert.Equal("Some error", apiResponse.Errors[0]);
//         }
//     }
// }

using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using eRM_VersionHub_Tester.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class AppsControllerTests : IAsyncLifetime
    {
        private TestFixture _factory;
        private HttpClient _client;
        private string appsPath, appJson, internalPath, externalPath;
        private readonly MyAppSettings _settings;
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

        private List<AppStructureDto> GetAppStructureFromFiles()
        {
            var appStructure = new List<AppStructureDto>();
            var appDirectories = Directory.GetDirectories(appsPath);

            return appStructure;
        }

        [Fact]
        public async Task GetStructure_ShouldReturnAppStructure()
        {
            // Arrange
            var userName = "testUser";
            var appStructure = GetAppStructureFromFiles();

            // Act
            var response = await _client.GetAsync($"/Apps/{userName}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<AppStructureDto>>>(responseContent);
            Assert.True(apiResponse.Success);
            Assert.Equal(appStructure.Count, apiResponse.Data.Count);
        }

        [Fact]
        public async Task GetStructure_ShouldReturnNotFound_WhenStructureIsEmpty()
        {
            // Arrange
            var userName = "testUser";
            Directory.Delete(appsPath, true);
            Directory.CreateDirectory(appsPath);

            // Act
            var response = await _client.GetAsync($"/Apps/{userName}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
            Assert.False(apiResponse.Success);
            Assert.Equal("Some error", apiResponse.Errors[0]);
        }

        [Fact]
        public async Task GetStructure_ShouldReturnNotFound_WhenStructureIsNull()
        {
            // Arrange
            var userName = "testUser";
            Directory.Delete(appsPath, true);

            // Act
            var response = await _client.GetAsync($"/Apps/{userName}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseContent);
            Assert.False(apiResponse.Success);
            Assert.Equal("Some error", apiResponse.Errors[0]);
        }
    }
}
