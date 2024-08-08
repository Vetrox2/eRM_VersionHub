using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class DbService : IDbService
    {
        private readonly ILogger<DbService> _logger;
        private readonly IDbRepository _repository;

        public DbService(IDbRepository repository, ILogger<DbService> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<T?> GetAsync<T>(string template, object parameters)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAsync of type {type}", type);

            try
            {
                var result = await _repository.GetAsync<T>(template, parameters);
                _logger.LogInformation(
                    AppLogEvents.Database,
                    "GetAsync of type {type} completed successfully",
                    type
                );
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error in GetAsync of type {type}",
                    type
                );
                throw; // Re-throw the exception to be handled by the global error handler
            }
        }

        public async Task<List<T>> GetAll<T>(string template, object parameters)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked GetAll of type {type}", type);

            try
            {
                var result = await _repository.GetAll<T>(template, parameters);
                _logger.LogInformation(
                    AppLogEvents.Database,
                    "GetAll of type {type} completed successfully",
                    type
                );
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(AppLogEvents.Database, ex, "Error in GetAll of type {type}", type);
                throw; // Re-throw the exception to be handled by the global error handler
            }
        }

        public async Task<T?> EditData<T>(string template, object parameters)
        {
            string type = typeof(T).Name;
            _logger.LogDebug(AppLogEvents.Database, "Invoked EditData of type {type}", type);

            try
            {
                var result = await _repository.EditData<T>(template, parameters);
                _logger.LogInformation(
                    AppLogEvents.Database,
                    "EditData of type {type} completed successfully",
                    type
                );
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    AppLogEvents.Database,
                    ex,
                    "Error in EditData of type {type}",
                    type
                );
                throw; // Re-throw the exception to be handled by the global error handler
            }
        }
    }
}
