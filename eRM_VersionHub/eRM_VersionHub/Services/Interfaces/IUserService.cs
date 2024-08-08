using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(User user);
        Task<List<User>> GetUserList();
        Task<List<string>> GetUserNamesList();
        Task<List<UserDto>> GetUsersWithApps();
        Task<User> GetUser(string Username);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(string Username);
    }
}
