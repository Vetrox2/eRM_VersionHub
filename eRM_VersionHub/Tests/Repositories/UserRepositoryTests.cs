using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using Moq;

namespace eRM_VersionHub_Tester.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<IDbRepository> _mockDbRepository;
        private readonly Mock<ILogger<UserRepository>> _mockLogger;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mockDbRepository = new Mock<IDbRepository>();
            _mockLogger = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_mockDbRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var result = await _userRepository.CreateUser(user);

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.CreationDate, result.CreationDate);
        }

        [Fact]
        public async Task CreateUser_ShouldThrowExceptionOnFailure()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userRepository.CreateUser(user)
            );
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
                .ReturnsAsync(users);

            var result = await _userRepository.GetUserList();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(users, result);
        }

        [Fact]
        public async Task GetUserList_ShouldThrowExceptionOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.GetAll<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((List<User>)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.GetUserList());
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.GetAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var result = await _userRepository.GetUser("testUser");

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.CreationDate, result.CreationDate);
        }

        [Fact]
        public async Task GetUser_ShouldThrowExceptionOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.GetAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.GetUser("testUser"));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var result = await _userRepository.UpdateUser(user);

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.CreationDate, result.CreationDate);
        }

        [Fact]
        public async Task UpdateUser_ShouldThrowExceptionOnFailure()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _userRepository.UpdateUser(user));
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnDeletedUser()
        {
            var user = new User { Username = "testUser", CreationDate = DateTime.Now };
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(user);

            var result = await _userRepository.DeleteUser("testUser");

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.CreationDate, result.CreationDate);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrowExceptionOnFailure()
        {
            _mockDbRepository
                .Setup(repo => repo.EditData<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _userRepository.DeleteUser("testUser")
            );
        }
    }
}
