using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Database;
using eRM_VersionHub.Repositories.Interfaces;
using Moq;

namespace eRM_VersionHub.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<IDbRepository> _mockDbRepository;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mockDbRepository = new Mock<IDbRepository>();
            _userRepository = new UserRepository(_mockDbRepository.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<User?>.SuccessResponse(user));

            var result = await _userRepository.CreateUser(user);

            Assert.True(result.Success);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnErrorOnFailure()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<User?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _userRepository.CreateUser(user);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task GetUserList_ShouldReturnListOfUsers()
        {
            var users = new List<User>
            {
                new User { Username = "user1", CreationDate = DateTime.Now },
                new User { Username = "user2", CreationDate = DateTime.Now }
            };
            _mockDbRepository
                .Setup(repo => repo.GetAll<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<List<User>>.SuccessResponse(users));

            var result = await _userRepository.GetUserList();

            Assert.True(result.Success);
            Assert.Equal(users, result.Data);
        }

        [Fact]
        public async Task GetUserList_ShouldReturnErrorOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.GetAll<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<List<User>>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _userRepository.GetUserList();

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.GetAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<User?>.SuccessResponse(user));

            var result = await _userRepository.GetUser("testUser");

            Assert.True(result.Success);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task GetUser_ShouldReturnErrorOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.GetAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<User?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _userRepository.GetUser("testUser");

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<User?>.SuccessResponse(user));

            var result = await _userRepository.UpdateUser(user);

            Assert.True(result.Success);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnErrorOnFailure()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<User?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _userRepository.UpdateUser(user);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnDeletedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<User?>.SuccessResponse(user));

            var result = await _userRepository.DeleteUser("testUser");

            Assert.True(result.Success);
            Assert.Equal(user, result.Data);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnErrorOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<User?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _userRepository.DeleteUser("testUser");

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }
    }
}
