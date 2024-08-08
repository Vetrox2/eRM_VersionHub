using eRM_VersionHub.Models;

namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<List<User>> GetUserList();
        Task<User> GetUser(string Username);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(string Username);
    }
}
