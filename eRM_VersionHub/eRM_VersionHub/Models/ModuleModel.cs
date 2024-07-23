using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class ModuleModel
    {
        public string Name { get; set; }
        public List<string> Versions { get; set; }
        public override string ToString() => JsonManager.Serialize<ModuleModel>(this);
    }
}
