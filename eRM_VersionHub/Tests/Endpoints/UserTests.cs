using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Database;
using eRM_VersionHub_Tester.Helpers;
using Moq;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class UserTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly User user;
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserTests(TestFixture factory)
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockRepository.Object, _mockLogger.Object);
            _client = factory.CreateClient();
            user = new() { Username = "test", CreationDate = DateTime.MinValue };
        }

        [Fact]
        public async Task GetUserList_ShouldReturnListOfUsers()
        {
            HttpResponseMessage response = await _client.GetAsync("/User");
            List<User>? deserialized = await response.GetRequestContent<List<User>?>();
            Assert.NotNull(deserialized);
            Assert.NotEmpty(deserialized);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            HttpResponseMessage response = await _client.GetAsync("/User/admin");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.NotNull(deserialized);
            Assert.Equal("admin", deserialized.Username);
        }

        [Fact]
        public async Task GetUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.GetAsync("/User/#");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<User>("/User", user);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.NotNull(deserialized);
            Assert.True(user.Equals(deserialized));
        }

        [Fact]
        public async Task CreateUser_ShouldReturnErrorOnFailure()
        {
            Favorite fav = new() 
            {
                Username = "", AppID = ""
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync<Favorite>("/User", fav);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            User updatedUser = new()
            {
                Username = user.Username,
                CreationDate = DateTime.MaxValue,
            };
            var expectedResponse = new ApiResponse<User?> { Errors = [], Data = user };
            _mockRepository.Setup(repo => repo.CreateUser(user)).ReturnsAsync(expectedResponse);
            HttpResponseMessage response = await _client.PutAsJsonAsync<User>("/User", updatedUser);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.NotNull(deserialized);
            Assert.True(updatedUser.Equals(deserialized));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnErrorOnFailure()
        {
            Favorite fav = new()
            {
                Username = "",
                AppID = ""
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync<Favorite>("/User", fav);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnDeletedUser()
        {
            var expectedResponse = new ApiResponse<User?> { Errors = [], Data = user };
            _mockRepository.Setup(repo => repo.CreateUser(user)).ReturnsAsync(expectedResponse);
            HttpResponseMessage response = await _client.DeleteAsync("/User/test");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.NotNull(deserialized);
            Assert.True(user.Equals(deserialized));
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.DeleteAsync("/User/#");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }
    }
}