
using eRM_VersionHub.Models;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class DbService(IDbRepository repository, ILogger<DbService> logger) : IDbService
    {
        private readonly ILogger<DbService> _logger = logger;
        private readonly IDbRepository _repository = repository;

        public async Task<ApiResponse<T?>> GetAsync<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAsync of type {type} with query: {command}, {parms}", type, command, parms);

            ApiResponse<T?> result = await _repository.GetAsync<T>(command, parms);
            _logger.LogInformation(AppLogEvents.Database, "GetAsync of type {type} returned: {result}", type, result);
            return result;
        }

        public async Task<ApiResponse<List<T>>> GetAll<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAll of type {type} with query: {command}, {parms}", type, command, parms);

            ApiResponse<List<T>> result = await _repository.GetAll<T>(command, parms);
            _logger.LogInformation(AppLogEvents.Database, "GetAll of type {type} returned: {result}", type, result);
            return result;
        }

        public async Task<ApiResponse<T?>> EditData<T>(string command, object parms)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked EditData of type {type} with query: {command}, {parms}", type, command, parms);

            ApiResponse<T?> result = await _repository.EditData<T>(command, parms);
            _logger.LogInformation(AppLogEvents.Database, "EditData of type {type} returned: {result}", type, result);
            return result;
        }
    }
}