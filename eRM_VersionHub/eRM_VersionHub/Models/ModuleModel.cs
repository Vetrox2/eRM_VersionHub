using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class ModuleModel
    {
        public required string Name { get; set; }
        public required List<string> Versions { get; set; }
        public override string ToString() => JsonManager.Serialize<ModuleModel>(this);
    }
}
