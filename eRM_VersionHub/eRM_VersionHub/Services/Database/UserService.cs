using eRM_VersionHub.Models;
using eRM_VersionHub.Result;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class UserService(IUserRepository repository) : IUserService
    {
        private readonly IUserRepository _repository = repository;

        public async Task<Result<User?>> CreateUser(User user)
        {
            return await _repository.CreateUser(user);
        }

        public async Task<Result<List<User>>> GetUserList()
        {
            return await _repository.GetUserList();
        }

        public async Task<Result<User?>> GetUser(string Username)
        {
            return await _repository.GetUser(Username);
        }

        public async Task<Result<User?>> UpdateUser(User user)
        {
            return await _repository.UpdateUser(user);
        }

        public async Task<Result<User?>> DeleteUser(string Username)
        {
            return await _repository.DeleteUser(Username);
        }
    }
}