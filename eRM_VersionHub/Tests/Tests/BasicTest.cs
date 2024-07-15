using eRM_VersionHub.Models;
using eRM_VersionHub.Services;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace eRM_VersionHub_Tester.Tests
{
    public class BasicTests(TestFixture factory) : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client = factory.CreateClient();

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
            User? user = JsonManager.Deserialize<User?>(result);
            Assert.NotNull(user);
            Assert.Equal("admin", user.Username);
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
                User? user = JsonManager.Deserialize<User?>(result);
            });
            Assert.Throws<JsonException>(action);
        }
    }
}