using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Database;
using Moq;

namespace eRM_VersionHub.Tests.Services
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _mockRepository;
        private readonly FavoriteService _favoriteService;

        public FavoriteServiceTests()
        {
            _mockRepository = new Mock<IFavoriteRepository>();
            _favoriteService = new FavoriteService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateFavorite_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var favorite = new Favorite { Username = username, AppID = "testApp" };
            var expectedResponse = new ApiResponse<Favorite?> { Errors = [], Data = favorite };
            _mockRepository
                .Setup(repo => repo.CreateFavorite(It.IsAny<Favorite>()))
                .ReturnsAsync(expectedResponse);

            var result = await _favoriteService.CreateFavorite(favorite);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.CreateFavorite(favorite), Times.Once);
        }

        [Fact]
        public async Task GetFavoriteList_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var expectedResponse = new ApiResponse<List<Favorite>>
            {
                Errors = [],
                Data = new List<Favorite>
                {
                    new Favorite { Username = username, AppID = "testApp" }
                }
            };
            _mockRepository
                .Setup(repo => repo.GetFavoriteList(username))
                .ReturnsAsync(expectedResponse);

            var result = await _favoriteService.GetFavoriteList(username);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.GetFavoriteList(username), Times.Once);
        }

        [Fact]
        public async Task DeleteFavorite_ShouldCallRepositoryMethod()
        {
            var username = "testUser";
            var favorite = new Favorite { Username = username, AppID = "testApp" };
            var expectedResponse = new ApiResponse<Favorite?> { Errors = [], Data = favorite };
            _mockRepository
                .Setup(repo => repo.DeleteFavorite(It.IsAny<Favorite>()))
                .ReturnsAsync(expectedResponse);

            var result = await _favoriteService.DeleteFavorite(favorite);

            Assert.Equal(expectedResponse, result);
            _mockRepository.Verify(repo => repo.DeleteFavorite(favorite), Times.Once);
        }
    }
}
