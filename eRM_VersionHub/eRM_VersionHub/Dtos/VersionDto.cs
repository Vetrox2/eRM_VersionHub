namespace eRM_VersionHub.Dtos
{
    public class VersionDto
    {
        public string ID {  get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public List<ModuleDto> Modules { get; set; }

        public VersionDto(string id, List<ModuleDto> modules)
        {
            ID = id;
            Modules = modules;
            var index = ID.IndexOf('-');

            if (index == -1)
            {
                Name = ID;
                Tag = string.Empty;
            }
            else
            {
                Name = ID.Substring(0, index);
                Tag = ID[(index + 1)..];
            }
        }
    }
}
