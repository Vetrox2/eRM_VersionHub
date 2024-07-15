using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using System.Text.Json;

namespace eRM_VersionHub_Tester.Tests
{
    public class BasicTests(TestFixture factory) : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client = factory.CreateClient();
        private readonly static User user = new()
        {
            Username = "test",
            CreationDate = DateTime.Now,
        };

        [Theory]
        [InlineData("/User")]
        public async Task GetUsers(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            List<User>? array = JsonManager.Deserialize<List<User>?>(result);
            Assert.NotNull(array);
            Assert.NotEmpty(array);
        }

        [Theory]
        [InlineData("/User/admin")]
        public async Task GetUser(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            User? returnedUser = JsonManager.Deserialize<User?>(result);
            Assert.NotNull(returnedUser);
            Assert.Equal("admin", returnedUser.Username);
        }

        [Theory]
        [InlineData("/User/#")]
        public async Task GetNonExistentUser(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            Action action = new(() =>
            {
                User? returnedUser = JsonManager.Deserialize<User?>(result);
            });
            Assert.Throws<JsonException>(action);
        }

        [Theory]
        [InlineData("/User/")]
        public async Task AddUser(string url)
        {
            
            HttpResponseMessage response = await _client.PostAsJsonAsync<User>(url, user);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            User? returnedUser = JsonManager.Deserialize<User?>(result);
            Assert.NotNull(returnedUser);
            Assert.Equal(user, returnedUser);
        }

        [Theory]
        [InlineData("/User/test")]
        public async Task DeleteUser(string url)
        {
            HttpResponseMessage response = await _client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            User? returnedUser = JsonManager.Deserialize<User?>(result);
            Assert.NotNull(returnedUser);
            Assert.Equal(user, returnedUser);
        }
    }
}
