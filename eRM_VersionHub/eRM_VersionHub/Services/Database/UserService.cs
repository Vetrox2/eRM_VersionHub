using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class UserService(IUserRepository repository, ILogger<UserService> logger) : IUserService
    {
        private readonly ILogger<UserService> _logger = logger;
        private readonly IUserRepository _repository = repository;

        public async Task<ApiResponse<User?>> CreateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked CreateUser with data: {user}", user);
            ApiResponse<User?> result = await _repository.CreateUser(user);

            _logger.LogInformation(AppLogEvents.Service, "CreateUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<List<User>>> GetUserList()
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetUserList");
            ApiResponse<List<User>> result = await _repository.GetUserList();

            _logger.LogInformation(AppLogEvents.Service, "GetUserList returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> GetUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetUser with parameter: {Username}", Username);
            ApiResponse<User?> result = await _repository.GetUser(Username);

            _logger.LogInformation(AppLogEvents.Service, "GetUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> UpdateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked UpdateUser with data: {user}", user);
            ApiResponse<User?> result = await _repository.UpdateUser(user);

            _logger.LogInformation(AppLogEvents.Service, "UpdateUser returned: {result}", result);
            return result;
        }

        public async Task<ApiResponse<User?>> DeleteUser(string Username)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked DeleteUser with parameter: {Username}", Username);
            ApiResponse<User?> result = await _repository.DeleteUser(Username);

            _logger.LogInformation(AppLogEvents.Service, "DeleteUser returned: {result}", result);
            return result;
        }
    }
}