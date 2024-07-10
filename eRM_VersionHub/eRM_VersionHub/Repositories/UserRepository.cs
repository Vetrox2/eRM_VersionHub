using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using eRM_VersionHub.Repositories.Interfaces;

namespace eRM_VersionHub.Repositories.Database
{
    public class UserRepository(IDbRepository dbRepository) : IUserRepository
    {
        private readonly IDbRepository _dbRepository = dbRepository;

        public async Task<Result<User?>> CreateUser(User user)
        {
            Result<User?> CreatedUser = await _dbRepository.EditData<User>(
                "INSERT INTO users(username, creation_date) VALUES (@Username, @CreationDate) RETURNING *", user);
            return CreatedUser;
        }

        public async Task<Result<List<User>>> GetUserList()
        {
            Result<List<User>> UserList = await _dbRepository.GetAll<User>("SELECT * FROM users", new { });
            return UserList;
        }

        public async Task<Result<User?>> GetUser(string Username)
        {
            Result<User?> UserList = await _dbRepository.GetAsync<User>(
                "SELECT * FROM users where username=@Username", new { Username });
            return UserList;
        }

        public async Task<Result<User?>> UpdateUser(User user)
        {
            Result<User?> UpdatedUser = await _dbRepository.EditData<User>(
                "UPDATE users SET creation_date=@CreationDate WHERE username=@Username RETURNING *", user);
            return UpdatedUser;
        }

        public async Task<Result<User?>> DeleteUser(string Username)
        {
            Result<User?> DeletedUser = await _dbRepository.EditData<User>(
                "DELETE FROM users WHERE username=@Username RETURNING *", new { Username });
            return DeletedUser;
        }
    }
}