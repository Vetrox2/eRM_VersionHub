using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Database;
using Moq;

namespace eRM_VersionHub_Tester.Services
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _mockRepository;
        private readonly Mock<ILogger<FavoriteService>> _mockLogger;
        private readonly FavoriteService _favoriteService;

        public FavoriteServiceTests()
        {
            _mockRepository = new Mock<IFavoriteRepository>();
            _mockLogger = new Mock<ILogger<FavoriteService>>();
            _favoriteService = new FavoriteService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateFavorite_ShouldCallRepositoryMethod()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockRepository
                .Setup(repo => repo.CreateFavorite(It.IsAny<Favorite>()))
                .ReturnsAsync(favorite);

            var result = await _favoriteService.CreateFavorite(favorite);

            Assert.Equal(favorite, result);
            _mockRepository.Verify(repo => repo.CreateFavorite(favorite), Times.Once);
        }

        [Fact]
        public async Task GetFavoriteList_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var favorites = new List<Favorite>
            {
                new Favorite { Username = username, AppID = "testApp" }
            };
            _mockRepository.Setup(repo => repo.GetFavoriteList(username)).ReturnsAsync(favorites);

            var result = await _favoriteService.GetFavoriteList(username);

            Assert.Equal(favorites, result);
            _mockRepository.Verify(repo => repo.GetFavoriteList(username), Times.Once);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldCallRepositoryMethod()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockRepository
                .Setup(repo => repo.DeleteFavorite(It.IsAny<Favorite>()))
                .ReturnsAsync(favorite);

            var result = await _favoriteService.DeleteFavorite(favorite);

            Assert.Equal(favorite, result);
            _mockRepository.Verify(repo => repo.DeleteFavorite(favorite), Times.Once);
        }
    }
}
