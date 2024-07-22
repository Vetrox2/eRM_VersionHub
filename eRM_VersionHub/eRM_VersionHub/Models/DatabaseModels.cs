using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class Permission
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
        public override string ToString() => JsonManager.Serialize<Permission>(this);
    }

    public class Favorite
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
        public override string ToString() => JsonManager.Serialize<Favorite>(this);
    }

    public class User
    {
        public required string Username { get; set; }
        public required DateTime CreationDate { get; set; }
        public override string ToString() => JsonManager.Serialize<User>(this);
    }
}
