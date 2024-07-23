using eRM_VersionHub.Services;

namespace eRM_VersionHub.Models
{
    public class Permission
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
        public override string ToString() => this.Serialize();
    }

    public class Favorite
    {
        public required string Username { get; set; }
        public required string AppID { get; set; }
        public override string ToString() => this.Serialize();
    }

    public class User
    {
        public required string Username { get; set; }
        public required DateTime CreationDate { get; set; }

        public override string ToString() => this.Serialize();
        public virtual bool Equals(User user) => Username == user.Username && CreationDate == user.CreationDate;
    }
}
