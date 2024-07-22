using System.Reflection;
using eRM_VersionHub.Services;

namespace eRM_VersionHub.Dtos
{
    public class VersionDto
    {
        public string ID {  get; set; }
        public string Number { get; set; }
        public string PublishedTag { get; set; }
        public List<ModuleDto> Modules { get; set; }

        public VersionDto(string id, List<ModuleDto> modules)
        {
            ID = id;
            Modules = modules;
            Number = TagService.GetVersionWithoutTag(ID);
            PublishedTag = "";
        }
    }
}
