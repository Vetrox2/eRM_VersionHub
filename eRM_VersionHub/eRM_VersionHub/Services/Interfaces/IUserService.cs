using eRM_VersionHub.Models;
using eRM_VersionHub.Result;

namespace eRM_VersionHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<User?>> CreateUser(User user);
        Task<Result<List<User>>> GetUserList();
        Task<Result<User?>> GetUser(string Username);
        Task<Result<User?>> UpdateUser(User user);
        Task<Result<User?>> DeleteUser(string Username);
    }
}