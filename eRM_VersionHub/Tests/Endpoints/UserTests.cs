using eRM_VersionHub.Models;
using eRM_VersionHub_Tester.Helpers;
using System.Text.Json;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class UserTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly User user;

        public UserTests(TestFixture factory)
        {
            _client = factory.CreateClient();
            user = new() { Username = "test", CreationDate = DateTime.Now };
        }

        [Fact]
        public async Task GetUserList_ShouldReturnListOfUsers()
        {
            HttpResponseMessage response = await _client.GetAsync("/User");
            Func<List<User>?> fun = await RequestContent.GetRequestContent<List<User>>(response);
            List<User>? deserialized = fun.Invoke();
            Assert.NotNull(deserialized);
            Assert.NotEmpty(deserialized);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            HttpResponseMessage response = await _client.GetAsync("/User/admin");
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            User? deserialized = fun.Invoke();
            Assert.NotNull(deserialized);
            Assert.Equal("admin", deserialized.Username);
        }

        [Fact]
        public async Task GetUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.GetAsync("/User/#");
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            Assert.Throws<JsonException>(fun);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedUser()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<User>("/User", user);
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            User? deserialized = fun.Invoke();
            Assert.NotNull(deserialized);
            Assert.Equal(user, deserialized);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnErrorOnFailure()
        {
            Favorite fav = new() 
            {
                Username = "", AppID = ""
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync<Favorite>("/User", fav);
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            Assert.Throws<JsonException>(fun);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            User updatedUser = new()
            {
                Username = user.Username,
                CreationDate = DateTime.MaxValue,
            };
            HttpResponseMessage response = await _client.PutAsJsonAsync<User>("/User", user);
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            User? deserialized = fun.Invoke();
            Assert.NotNull(deserialized);
            Assert.Equal(user, deserialized);
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
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            Assert.Throws<JsonException>(fun);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnDeletedUser()
        {
            HttpResponseMessage response = await _client.DeleteAsync("/User/test");
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            User? deserialized = fun.Invoke();
            Assert.NotNull(deserialized);
            Assert.Equal(user, deserialized);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.DeleteAsync("/User/#");
            Func<User?> fun = await RequestContent.GetRequestContent<User?>(response);
            Assert.Throws<JsonException>(fun);
        }
    }
}