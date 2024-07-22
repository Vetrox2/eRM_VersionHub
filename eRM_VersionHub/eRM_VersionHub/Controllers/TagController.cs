using Microsoft.AspNetCore.Mvc;
using eRM_VersionHub.Models;
using Microsoft.Extensions.Options;
using eRM_VersionHub.Services.Interfaces;

namespace eRM_VersionHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TagController(IOptions<AppSettings> appSettings, ITagService tagService) : ControllerBase
    {
        private readonly MyAppSettings _settings = appSettings.Value.MyAppSettings;
        private readonly ITagService _tagService = tagService;

        [HttpPost]
        public IActionResult SetTag(string appID, string versionID, string newTag= "")
        {
            var response = _tagService.SetTag(_settings, appID, versionID, newTag);

            return response.Success ? Ok(response) : NotFound(response);
        }
    }
}
