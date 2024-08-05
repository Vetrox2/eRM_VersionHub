using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;


namespace eRM_VersionHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<User?>> CreateUser(User user);
        Task<ApiResponse<List<User>>> GetUserList();
        Task<ApiResponse<User?>> GetUser(string Username);
        Task<ApiResponse<User?>> UpdateUser(User user);
        Task<ApiResponse<User?>> DeleteUser(string Username);
        Task<ApiResponse<List<UserDto>>> GetUsersWithApps();
        Task<ApiResponse<List<string>>> GetUserNamesList();

    }
}