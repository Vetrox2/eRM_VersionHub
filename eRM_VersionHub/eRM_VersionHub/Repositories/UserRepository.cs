using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IDbRepository _dbRepository;

        public UserRepository(IDbRepository dbRepository, ILogger<UserRepository> logger)
        {
            _logger = logger;
            _dbRepository = dbRepository;
        }

        public async Task<User> CreateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked CreateUser with data: {user}", user);
            var result = await _dbRepository.EditData<User>(
                "INSERT INTO users(username, creation_date) VALUES (@Username, @CreationDate) RETURNING *",
                user
            );
            if (result == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }
            _logger.LogInformation(AppLogEvents.Repository, "CreateUser completed successfully");
            return result;
        }

        public async Task<List<User>> GetUserList()
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked GetUserList");
            var result = await _dbRepository.GetAll<User>("SELECT * FROM users", new { });
            if (result == null || !result.Any())
            {
                throw new NotFoundException("No users found");
            }
            _logger.LogInformation(AppLogEvents.Repository, "GetUserList completed successfully");
            return result;
        }

        public async Task<User> GetUser(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Repository,
                "Invoked GetUser with data: {Username}",
                Username
            );
            var result = await _dbRepository.GetAsync<User>(
                "SELECT * FROM users where username=@Username",
                new { Username }
            );
            if (result == null)
            {
                throw new NotFoundException($"User with username {Username} not found");
            }
            _logger.LogInformation(AppLogEvents.Repository, "GetUser completed successfully");
            return result;
        }

        public async Task<User> UpdateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Repository, "Invoked UpdateUser with data: {user}", user);
            var result = await _dbRepository.EditData<User>(
                "UPDATE users SET creation_date=@CreationDate WHERE username=@Username RETURNING *",
                user
            );
            if (result == null)
            {
                throw new NotFoundException($"User with username {user.Username} not found");
            }
            _logger.LogInformation(AppLogEvents.Repository, "UpdateUser completed successfully");
            return result;
        }

        public async Task<User> DeleteUser(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Repository,
                "Invoked DeleteUser with data: {Username}",
                Username
            );
            var result = await _dbRepository.EditData<User>(
                "DELETE FROM users WHERE username=@Username RETURNING *",
                new { Username }
            );
            if (result == null)
            {
                throw new NotFoundException($"User with username {Username} not found");
            }
            _logger.LogInformation(AppLogEvents.Repository, "DeleteUser completed successfully");
            return result;
        }
    }
}
