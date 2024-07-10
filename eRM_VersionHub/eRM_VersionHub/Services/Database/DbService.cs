﻿using eRM_VersionHub.Result;
using eRM_VersionHub.Repositories.Interfaces;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Services.Database
{
    public class DbService(IDbRepository repository) : IDbService
    {
        private readonly IDbRepository _repository = repository;

        public async Task<Result<T?>> GetAsync<T>(string command, object parms)
        {
            return await _repository.GetAsync<T>(command, parms);
        }

        public async Task<Result<List<T>>> GetAll<T>(string command, object parms)
        {
            return await _repository.GetAll<T>(command, parms);
        }

        public async Task<Result<T?>> EditData<T>(string command, object parms)
        {
            return await _repository.EditData<T>(command, parms);
        }
    }
}
