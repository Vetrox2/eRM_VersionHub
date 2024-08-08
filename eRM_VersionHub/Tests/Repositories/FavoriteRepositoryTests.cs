using eRM_VersionHub.Models;
using Moq;

namespace eRM_VersionHub_Tester.Repositories
{
    public class FavoriteRepositoryTests
    {
        private readonly Mock<IDbRepository> _mockDbRepository;
        private readonly Mock<ILogger<FavoriteRepository>> _mockLogger;
        private readonly FavoriteRepository _favoriteRepository;

        public FavoriteRepositoryTests()
        {
            _mockDbRepository = new Mock<IDbRepository>();
            _mockLogger = new Mock<ILogger<FavoriteRepository>>();
            _favoriteRepository = new FavoriteRepository(
                _mockDbRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateFavorite_ShouldReturnCreatedFavorite()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(favorite);

            var result = await _favoriteRepository.CreateFavorite(favorite);

            Assert.Equal(favorite, result);
        }

        [Fact]
        public async Task GetFavoriteList_ShouldReturnListOfFavorites()
        {
            var username = "testUser";
            var favorites = new List<Favorite>
            {
                new Favorite { Username = username, AppID = "app1" },
                new Favorite { Username = username, AppID = "app2" }
            };
            _mockDbRepository
                .Setup(repo => repo.GetAll<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(favorites);

            var result = await _favoriteRepository.GetFavoriteList(username);

            Assert.Equal(favorites, result);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnDeletedFavorite()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(favorite);

            var result = await _favoriteRepository.DeleteFavorite(favorite);

            Assert.Equal(favorite, result);
        }

        [Fact]
        public async Task CreateFavorite_ShouldReturnErrorOnFailure()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Favorite)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _favoriteRepository.CreateFavorite(favorite)
            );

            _mockLogger.Verify(
                x =>
                    x.Log(
                        LogLevel.Debug,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>(
                            (o, t) => o.ToString().Contains("Invoked CreateFavorite")
                        ),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnErrorOnFailure()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync((Favorite)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _favoriteRepository.CreateFavorite(favorite)
            );

            _mockLogger.Verify(
                x =>
                    x.Log(
                        LogLevel.Debug,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>(
                            (o, t) => o.ToString().Contains("Invoked CreateFavorite")
                        ),
                        It.IsAny<Exception>(),
                        It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                    ),
                Times.Once
            );
        }
    }
}
