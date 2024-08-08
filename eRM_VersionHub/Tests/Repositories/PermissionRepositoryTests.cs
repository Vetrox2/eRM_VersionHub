using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using Moq;

namespace eRM_VersionHub_Tester.Repositories
{
    public class PermissionRepositoryTests
    {
        private readonly Mock<IDbRepository> _mockDbRepository;
        private readonly Mock<ILogger<PermissionRepository>> _mockLogger;
        private readonly PermissionRepository _permissionRepository;

        public PermissionRepositoryTests()
        {
            _mockDbRepository = new Mock<IDbRepository>();
            _mockLogger = new Mock<ILogger<PermissionRepository>>();
            _permissionRepository = new PermissionRepository(
                _mockDbRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreatePermission_ShouldReturnCreatedPermission()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(permission);

            // Act
            var result = await _permissionRepository.CreatePermission(permission);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(permission.Username, result.Username);
            Assert.Equal(permission.AppID, result.AppID);
        }

        [Fact]
        public async Task CreatePermission_ShouldThrowExceptionOnFailure()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Permission)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _permissionRepository.CreatePermission(permission)
            );
        }

        [Fact]
        public async Task GetPermissionList_ShouldReturnListOfPermissions()
        {
            // Arrange
            var username = "testUser";
            var permissions = new List<Permission>
            {
                new Permission { Username = username, AppID = "app1" },
                new Permission { Username = username, AppID = "app2" }
            };
            _mockDbRepository
                .Setup(repo => repo.GetAll<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(permissions);

            // Act
            var result = await _permissionRepository.GetPermissionList(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(permissions, result);
        }

        [Fact]
        public async Task GetPermissionList_ShouldThrowExceptionOnFailure()
        {
            // Arrange
            var username = "testUser";
            _mockDbRepository
                .Setup(repo => repo.GetAll<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((List<Permission>)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _permissionRepository.GetPermissionList(username)
            );
        }

        [Fact]
        public async Task DeletePermission_ShouldReturnDeletedPermission()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(permission);

            // Act
            var result = await _permissionRepository.DeletePermission(permission);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(permission.Username, result.Username);
            Assert.Equal(permission.AppID, result.AppID);
        }

        [Fact]
        public async Task DeletePermission_ShouldThrowExceptionOnFailure()
        {
            // Arrange
            var permission = new Permission { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Permission>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Permission)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _permissionRepository.DeletePermission(permission)
            );
        }
    }
}
