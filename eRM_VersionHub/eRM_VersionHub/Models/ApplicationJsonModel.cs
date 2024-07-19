namespace eRM_VersionHub.Models
{
    public class ApplicationJsonModel
    {
        public string UniqueIdentifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ModuleJsonModel> Modules { get; set; }

        public List<string> GetModulesNames()
            => Modules != null ? Modules.Select(module => module.ModuleId).ToList() : [];

    }
    public class ModuleJsonModel
    {
        public string ModuleId { get; set; }
        public bool Optional { get; set; }

    }
}
