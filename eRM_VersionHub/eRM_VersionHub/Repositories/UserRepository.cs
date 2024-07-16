using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories
{
    public class UserRepository(IDbRepository dbRepository) : IUserRepository
    {
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<ApiResponse<User?>> CreateUser(User user)
        {
            ApiResponse<User?> CreatedUser = await _dbRepository.EditData<User>(
                "INSERT INTO users(username, creation_date) VALUES (@Username, @CreationDate) RETURNING *", user);
            return CreatedUser;
        }

        public async Task<ApiResponse<List<User>>> GetUserList()
        {
            ApiResponse<List<User>> UserList = await _dbRepository.GetAll<User>("SELECT * FROM users", new { });
            return UserList;
        }

        public async Task<ApiResponse<User?>> GetUser(string Username)
        {
            ApiResponse<User?> UserList = await _dbRepository.GetAsync<User>(
                "SELECT * FROM users where username=@Username", new { Username });
            return UserList;
        }

        public async Task<ApiResponse<User?>> UpdateUser(User user)
        {
            ApiResponse<User?> UpdatedUser = await _dbRepository.EditData<User>(
                "UPDATE users SET creation_date=@CreationDate WHERE username=@Username RETURNING *", user);
            return UpdatedUser;
        }

        public async Task<ApiResponse<User?>> DeleteUser(string Username)
        {
            ApiResponse<User?> DeletedUser = await _dbRepository.EditData<User>(
                "DELETE FROM users WHERE username=@Username RETURNING *", new { Username });
            return DeletedUser;
        }
    }
}