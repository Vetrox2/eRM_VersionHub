using eRM_VersionHub.Services;

namespace eRM_VersionHub.Dtos
{
    public class AppStructureDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsFavourite { get; set; }
        public List<VersionDto> Versions { get; set; }
        public override string ToString() => JsonManager.Serialize<AppStructureDto>(this);
    }
}
