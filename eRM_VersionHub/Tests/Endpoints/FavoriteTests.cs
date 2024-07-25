using eRM_VersionHub.Models;
using eRM_VersionHub_Tester.Helpers;

namespace eRM_VersionHub_Tester.Endpoints
{
    public class FavoriteTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly Favorite fav;
        private readonly User user;

        public FavoriteTests(TestFixture factory)
        {
            factory.SetNewAppSettings(string.Empty, string.Empty, string.Empty, string.Empty);
            _client = factory.CreateClient();
            fav = new() { Username = "admin", AppID = "app0" };
            user = new() { Username = "", CreationDate = DateTime.MinValue };
        }

        [Fact]
        public async Task GetFavoriteList_ShouldReturnListOfFavorites()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Favorite/admin");
            List<Favorite>? deserialized = await response.GetRequestContent<List<Favorite>?>();

            Assert.NotNull(deserialized);
            Assert.NotEmpty(deserialized);
        }

        [Fact]
        public async Task GetFavoriteList_ShouldReturnErrorOnFailure()
        {
            HttpResponseMessage response = await _client.GetAsync("api/Favorite/#");
            List<Favorite>? deserialized = await response.GetRequestContent<List<Favorite>?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task AddFavorite2_ShouldReturnFavorite()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<Favorite>("api/Favorite", fav);
            Favorite? deserialized = await response.GetRequestContent<Favorite?>();

            Assert.NotNull(deserialized);
            Assert.True(fav.Equals(deserialized));
        }

        [Fact]
        public async Task AddFavorite1_ShouldReturnErrorOnFailure_InvalidJSON()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<User>("api/Favorite", user);
            Favorite? deserialized = await response.GetRequestContent<Favorite?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnDeletedUser()
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/Favorite/{fav.Username}/{fav.AppID}");
            Favorite? deserialized = await response.GetRequestContent<Favorite?>();

            Assert.NotNull(deserialized);
            Assert.True(fav.Equals(deserialized));
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnErrorOnFailure_DeletingNonExistentFavorite()
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/Favorite/{user.Username}/{user.CreationDate}");
            Favorite? deserialized = await response.GetRequestContent<Favorite?>();
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnErrorOnFailure_InvalidJSON()
        {
            HttpResponseMessage response = await _client.DeleteAsync($"api/Favorite/{fav.Username}/{fav.AppID}");
            Favorite? deserialized = await response.GetRequestContent<Favorite?>();
            Assert.Null(deserialized);
        }
    }
}
