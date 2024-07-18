using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub_Tester.Helpers;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class UserTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly User user;
        private UserRepository userRepository;

        public UserTests(TestFixture factory)
        {
            factory.SetNewAppSettings("","","","");
            IDbRepository dbRepository = factory.GetIDbRepository();
            userRepository = new UserRepository(dbRepository);
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
            await userRepository.CreateUser(user);
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
            await userRepository.CreateUser(user);
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