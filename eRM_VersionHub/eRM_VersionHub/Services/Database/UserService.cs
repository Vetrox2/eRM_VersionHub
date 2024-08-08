using eRM_VersionHub.Dtos;
using eRM_VersionHub.Middleware;
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _repository;
        private readonly IPermissionService _permissionService;

        public UserService(
            IUserRepository repository,
            ILogger<UserService> logger,
            IPermissionService permissionService
        )
        {
            _logger = logger;
            _repository = repository;
            _permissionService = permissionService;
        }

        public async Task<User> CreateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked CreateUser with data: {user}", user);
            var result = await _repository.CreateUser(user);
            if (result == null)
            {
                throw new BadRequestException("Failed to create user");
            }
            _logger.LogInformation(AppLogEvents.Service, "CreateUser returned: {result}", result);
            return result;
        }

        public async Task<List<User>> GetUserList()
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetUserList");
            var result = await _repository.GetUserList();
            if (result == null || !result.Any())
            {
                throw new NotFoundException("No users found");
            }
            _logger.LogInformation(AppLogEvents.Service, "GetUserList returned: {result}", result);
            return result;
        }

        public async Task<List<string>> GetUserNamesList()
        {
            var users = await _repository.GetUserList();
            return users.Select(u => u.Username).ToList();
        }

        public async Task<List<UserDto>> GetUsersWithApps()
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked GetUserList");
            var users = await _repository.GetUserList();
            if (users == null || !users.Any())
            {
                throw new NotFoundException("No users found");
            }
            _logger.LogInformation(AppLogEvents.Service, "GetUserList returned: {result}", users);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var perms = await _permissionService.GetPermissionList(user.Username);
                userDtos.Add(
                    new UserDto
                    {
                        Username = user.Username,
                        AppsNames = perms.Select(perm => perm.AppID).Distinct().ToList()
                    }
                );
            }
            return userDtos;
        }

        public async Task<User> GetUser(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked GetUser with parameter: {Username}",
                Username
            );
            var result = await _repository.GetUser(Username);
            if (result == null)
            {
                throw new NotFoundException($"User not found: {Username}");
            }
            _logger.LogInformation(AppLogEvents.Service, "GetUser returned: {result}", result);
            return result;
        }

        public async Task<User> UpdateUser(User user)
        {
            _logger.LogDebug(AppLogEvents.Service, "Invoked UpdateUser with data: {user}", user);
            var result = await _repository.UpdateUser(user);
            if (result == null)
            {
                throw new NotFoundException($"User not found: {user.Username}");
            }
            _logger.LogInformation(AppLogEvents.Service, "UpdateUser returned: {result}", result);
            return result;
        }

        public async Task<User> DeleteUser(string Username)
        {
            _logger.LogDebug(
                AppLogEvents.Service,
                "Invoked DeleteUser with parameter: {Username}",
                Username
            );
            var result = await _repository.DeleteUser(Username);
            if (result == null)
            {
                throw new NotFoundException($"User not found: {Username}");
            }
            _logger.LogInformation(AppLogEvents.Service, "DeleteUser returned: {result}", result);
            return result;
        }
    }
}
