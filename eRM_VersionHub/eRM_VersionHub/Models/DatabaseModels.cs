namespace eRM_VersionHub.Models
{
    public class Permission
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
    }

    public class Favorite
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
    }

    public class User
    {
        public required string Username { get; set; }
        public required DateTime CreationDate { get; set; }
    }
}
