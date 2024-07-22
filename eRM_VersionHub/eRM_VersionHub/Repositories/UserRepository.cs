using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class UserRepository(IDbRepository dbRepository, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger = logger;
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<ApiResponse<User?>> CreateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked CreateUser with data: {user}", user);
            ApiResponse<User?> result = await _dbRepository.EditData<User>(
                "INSERT INTO users(username, creation_date) VALUES (@Username, @CreationDate) RETURNING *", user);
            _logger.LogInformation(AppLogEvents.Repository, "CreateUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<User>>> GetUserList()
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetUserList");
            ApiResponse<List<User>> result = await _dbRepository.GetAll<User>("SELECT * FROM users", new { });
            _logger.LogInformation(AppLogEvents.Repository, "GetUserList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> GetUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetUser with data: {Username}", Username);
            ApiResponse<User?> result = await _dbRepository.GetAsync<User>(
                "SELECT * FROM users where username=@Username", new { Username });
            _logger.LogInformation(AppLogEvents.Repository, "GetUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> UpdateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked UpdateUser with data: {user}", user);
            ApiResponse<User?> result = await _dbRepository.EditData<User>(
                "UPDATE users SET creation_date=@CreationDate WHERE username=@Username RETURNING *", user);
            _logger.LogInformation(AppLogEvents.Repository, "UpdateUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> DeleteUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked DeleteUser with data: {Username}", Username);
            ApiResponse<User?> result = await _dbRepository.EditData<User>(
                "DELETE FROM users WHERE username=@Username RETURNING *", new { Username });
            _logger.LogInformation(AppLogEvents.Repository, "DeleteUser returned: {result}", result);
            return result;
        }
    }
}