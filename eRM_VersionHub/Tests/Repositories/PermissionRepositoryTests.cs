using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using eRM_VersionHub.Repositories.Interfaces;
using Moq;

namespace eRM_VersionHub_Tester.Repositories
{
    public class PermissionRepositoryTests
    {
        private readonly Mock<IDbRepository> _mockDbRepository;
        private readonly PermissionRepository _permissionRepository;

        public PermissionRepositoryTests()
        {
            _mockDbRepository = new Mock<IDbRepository>();
            _permissionRepository = new PermissionRepository(_mockDbRepository.Object);
        }

        [Fact]
        public async Task CreatePermission_ShouldReturnCreatedPermission()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<Permission?>.SuccessResponse(permission));

            var result = await _permissionRepository.CreatePermission(permission);

            Assert.True(result.Success);
            Assert.Equal(permission, result.Data);
        }

        [Fact]
        public async Task CreatePermission_ShouldReturnErrorOnFailure()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<Permission?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _permissionRepository.CreatePermission(permission);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task GetPermissionList_ShouldReturnListOfPermissions()
        {
            var username = "testUser";
            var permissions = new List<Permission>
            {
                new Permission { Username = username, AppID = "app1" },
                new Permission { Username = username, AppID = "app2" }
            };
            _mockDbRepository
                .Setup(repo => repo.GetAll<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<List<Permission>>.SuccessResponse(permissions));

            var result = await _permissionRepository.GetPermissionList(username);

            Assert.True(result.Success);
            Assert.Equal(permissions, result.Data);
        }

        [Fact]
        public async Task GetPermissionList_ShouldReturnErrorOnFailure()
        {
            var username = "testUser";
            _mockDbRepository
                .Setup(repo => repo.GetAll<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<List<Permission>>.ErrorResponse(
                        new List<string> { "Database error" }
                    )
                );

            var result = await _permissionRepository.GetPermissionList(username);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task DeletePermission_ShouldReturnDeletedPermission()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<Permission?>.SuccessResponse(permission));

            var result = await _permissionRepository.DeletePermission(permission);

            Assert.True(result.Success);
            Assert.Equal(permission, result.Data);
        }

        [Fact]
        public async Task DeletePermission_ShouldReturnErrorOnFailure()
        {
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<Permission?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _permissionRepository.DeletePermission(permission);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }
    }
}
