using eRM_VersionHub.Models;


namespace eRM_VersionHub.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<ApiResponse<User?>> CreateUser(User user);
        Task<ApiResponse<List<User>>> GetUserList();
        Task<ApiResponse<User?>> GetUser(string Username);
        Task<ApiResponse<User?>> UpdateUser(User user);
        Task<ApiResponse<User?>> DeleteUser(string Username);
    }
}