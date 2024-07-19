﻿using eRM_VersionHub.Dtos;
using eRM_VersionHub.Models;
using eRM_VersionHub.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicationController(IOptions<AppSettings> appSettings, IPublicationService publicationService) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly IPublicationService _publicationService = publicationService;


        [HttpPost]
        public ApiResponse<bool> Post(List<VersionDto> versionDtos)
        {
            if(versionDtos == null || versionDtos.Count == 0)
                return ApiResponse<bool>.ErrorResponse(["Empty collection of versions to publish"]);

            List<string> result = [];
            foreach (var version in versionDtos)
            {
                List<string> errors = _publicationService.Publish(_settings, version).Errors;
                result.AddRange(errors);
            }
            return ApiResponse<bool>.ErrorResponse(result);
        }

        [HttpDelete]
        public ApiResponse<bool> Delete(List<VersionDto> versionDtos)
        {
            if (versionDtos == null || versionDtos.Count == 0)
                return ApiResponse<bool>.ErrorResponse(["Empty collection of versions to unpublish"]);

            List<string> result = [];
            foreach (var version in versionDtos)
            {
                List<string> modules = _publicationService.Unpublish(_settings, version).Errors;
                result.AddRange(modules);
            }
            return ApiResponse<bool>.ErrorResponse(result);
        }
    }
}
