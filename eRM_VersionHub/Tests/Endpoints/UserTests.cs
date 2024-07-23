using eRM_VersionHub.Models;
using eRM_VersionHub_Tester.Helpers;
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace eRM_VersionHub_Tester.Endpoints
{
    public class UserTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly User user, updatedUser;
        private readonly Favorite fav;

        public UserTests(TestFixture factory)
        {
            factory.SetNewAppSettings(string.Empty, string.Empty, string.Empty, string.Empty);
            _client = factory.CreateClient();
            user = new() { Username = "test", CreationDate = DateTime.MinValue };
            updatedUser = new() { Username = user.Username, CreationDate = DateTime.MaxValue };
            fav = new() { Username = "", AppID = "" };
        }

        [Fact]
        public async Task GetUserList_ShouldReturnListOfUsers()
        {
            HttpResponseMessage response = await _client.GetAsync("api/User");
            List<User>? deserialized = await response.GetRequestContent<List<User>?>();

            Assert.NotNull(deserialized);
            Assert.NotEmpty(deserialized);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            HttpResponseMessage response = await _client.GetAsync("api/User/admin");
            User? deserialized = await response.GetRequestContent<User?>();

            Assert.NotNull(deserialized);
            Assert.Equal("admin", deserialized.Username);
        }

        [Fact]
        public async Task GetUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.GetAsync("api/User/#");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<User>("api/User", user);
            User? deserialized = await response.GetRequestContent<User?>();

            Assert.NotNull(deserialized);
            Assert.True(user.Equals(deserialized));
        }

        [Fact]
        public async Task CreateUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<Favorite>("api/User", fav);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync<User>("api/User", updatedUser);
            User? deserialized = await response.GetRequestContent<User?>();

            Assert.NotNull(deserialized);
            Assert.True(updatedUser.Equals(deserialized));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync<Favorite>("api/User", fav);
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnDeletedUser()
        {
            HttpResponseMessage response = await _client.DeleteAsync("api/User/test");
            User? deserialized = await response.GetRequestContent<User?>();

            Assert.NotNull(deserialized);
            Assert.True(updatedUser.Equals(deserialized));
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.DeleteAsync("api/User/#");
            User? deserialized = await response.GetRequestContent<User?>();
            Assert.Null(deserialized);
        }
    }
}