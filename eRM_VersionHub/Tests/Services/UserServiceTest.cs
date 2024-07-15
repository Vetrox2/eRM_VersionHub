using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Database;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldCallRepositoryMethod()
        {
            var username = "testUser";

            var user = new User { Username = username, CreationDate = DateTime.UtcNow };
            var expectedResponse = new ApiResponse<User?> { Errors = [], Data = user };
            _mockRepository
                .Setup(repo => repo.CreateUser(It.IsAny<User>()))
                .ReturnsAsync(expectedResponse);

            var result = await _userService.CreateUser(user);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.CreateUser(user), Times.Once);
        }

        [Fact]
        public async Task GetUserList_ShouldCallRepositoryMethod()
        {
            var username = "testUser";

            var expectedResponse = new ApiResponse<List<User>>
            {
                Errors = [],
                Data = new List<User>
                {
                    new User { Username = username, CreationDate = DateTime.UtcNow }
                }
            };
            _mockRepository.Setup(repo => repo.GetUserList()).ReturnsAsync(expectedResponse);

            var result = await _userService.GetUserList();

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.GetUserList(), Times.Once);
        }

        [Fact]
        public async Task GetUser_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var expectedResponse = new ApiResponse<User?>
            {
                Errors = [],
                Data = new User { Username = username, CreationDate = DateTime.UtcNow }
            };
            _mockRepository.Setup(repo => repo.GetUser(username)).ReturnsAsync(expectedResponse);

            var result = await _userService.GetUser(username);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.GetUser(username), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var user = new User { Username = username, CreationDate = DateTime.UtcNow };
            var expectedResponse = new ApiResponse<User?> { Errors = [], Data = user };
            _mockRepository
                .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
                .ReturnsAsync(expectedResponse);

            var result = await _userService.UpdateUser(user);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var expectedResponse = new ApiResponse<User?>
            {
                Errors = [],
                Data = new User { Username = username, CreationDate = DateTime.UtcNow }
            };
            _mockRepository.Setup(repo => repo.DeleteUser(username)).ReturnsAsync(expectedResponse);

            var result = await _userService.DeleteUser(username);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.DeleteUser(username), Times.Once);
        }
    }
}
