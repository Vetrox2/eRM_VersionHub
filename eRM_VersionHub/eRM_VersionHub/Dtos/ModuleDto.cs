using eRM_VersionHub.Services;

namespace eRM_VersionHub.Dtos
{
    public class ModuleDto
    {
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public bool IsOptional { get; set; }
        public override string ToString() => this.Serialize();
    }
}
