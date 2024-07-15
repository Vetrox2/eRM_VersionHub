using Microsoft.AspNetCore.Mvc.Testing;
using eRM_VersionHub.Services;
using eRM_VersionHub.Models;
using System.Text.Json;

namespace eRM_VersionHub_Tester.Tests
{
    public class TextFixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        { 
            builder.UseEnvironment("Testing");
            base.ConfigureWebHost(builder);
        }
    }

    public class BasicTests : IClassFixture<TextFixture>
    {
        private HttpClient HttpClient { get; }

        public BasicTests(TextFixture fixture)
        {
            WebApplicationFactoryClientOptions webAppFactoryClientOptions = new()
            {
                AllowAutoRedirect = false
            };
            HttpClient = fixture.CreateClient(webAppFactoryClientOptions);
            Thread.Sleep(200);
        }

        [Theory]
        [InlineData("/User")]
        public async Task GetUsers(string url)
        {  
            HttpResponseMessage response = await HttpClient.GetAsync(url);
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
            HttpResponseMessage response = await HttpClient.GetAsync(url);
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
            HttpResponseMessage response = await HttpClient.GetAsync(url);
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
