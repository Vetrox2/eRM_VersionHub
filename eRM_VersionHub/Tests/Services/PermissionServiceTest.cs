using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Database;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<IPermissionRepository> _mockRepository;
        private readonly Mock<ILogger<PermissionService>> _mockLogger;
        private readonly PermissionService _permissionService;

        public PermissionServiceTests()
        {
            _mockRepository = new Mock<IPermissionRepository>();
            _mockLogger = new Mock<ILogger<PermissionService>>();
            _permissionService = new PermissionService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreatePermission_ShouldCallRepositoryMethod()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            var expectedResponse = new ApiResponse<Permission?> { Errors = [], Data = permission };
            _mockRepository
                .Setup(repo => repo.CreatePermission(It.IsAny<Permission>()))
                .ReturnsAsync(expectedResponse);

            var result = await _permissionService.CreatePermission(permission);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.CreatePermission(permission), Times.Once);
        }

        [Fact]
        public async Task GetPermissionList_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var expectedResponse = new ApiResponse<List<Permission>>
            {
                Errors = [],
                Data = new List<Permission>
                {
                    new Permission { Username = username, AppID = "testApp" }
                }
            };
            _mockRepository
                .Setup(repo => repo.GetPermissionList(username))
                .ReturnsAsync(expectedResponse);

            var result = await _permissionService.GetPermissionList(username);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.GetPermissionList(username), Times.Once);
        }

        [Fact]
        public async Task DeletePermission_ShouldCallRepositoryMethod()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            var expectedResponse = new ApiResponse<Permission?> { Errors = [], Data = permission };
            _mockRepository
                .Setup(repo => repo.DeletePermission(It.IsAny<Permission>()))
                .ReturnsAsync(expectedResponse);

            var result = await _permissionService.DeletePermission(permission);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.DeletePermission(permission), Times.Once);
        }
    }
}
