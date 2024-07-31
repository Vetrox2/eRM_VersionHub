using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories;
using eRM_VersionHub.Repositories.Interfaces;
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
            _favoriteRepository = new FavoriteRepository(_mockDbRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateFavorite_ShouldReturnCreatedFavorite()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<Favorite?>.SuccessResponse(favorite));

            var result = await _favoriteRepository.CreateFavorite(favorite);

            Assert.True(result.Success);
            Assert.Equal(favorite, result.Data);
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
                .ReturnsAsync(ApiResponse<List<Favorite>>.SuccessResponse(favorites));

            var result = await _favoriteRepository.GetFavoriteList(username);

            Assert.True(result.Success);
            Assert.Equal(favorites, result.Data);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnDeletedFavorite()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(ApiResponse<Favorite?>.SuccessResponse(favorite));

            var result = await _favoriteRepository.DeleteFavorite(favorite);

            Assert.True(result.Success);
            Assert.Equal(favorite, result.Data);
        }

        [Fact]
        public async Task CreateFavorite_ShouldReturnErrorOnFailure()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<Favorite?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _favoriteRepository.CreateFavorite(favorite);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task GetFavoriteList_ShouldReturnErrorOnFailure()
        {
            var username = "testUser";
            _mockDbRepository
                .Setup(repo => repo.GetAll<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<List<Favorite>>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _favoriteRepository.GetFavoriteList(username);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldReturnErrorOnFailure()
        {
            var favorite = new Favorite { Username = "testUser", AppID = "testApp" };
            _mockDbRepository
                .Setup(repo => repo.EditData<Favorite>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(
                    ApiResponse<Favorite?>.ErrorResponse(new List<string> { "Database error" })
                );

            var result = await _favoriteRepository.DeleteFavorite(favorite);

            Assert.False(result.Success);
            Assert.Contains("Database error", result.Errors);
        }
    }
}
