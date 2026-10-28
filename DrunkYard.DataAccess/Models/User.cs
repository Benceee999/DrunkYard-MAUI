using SQLite;

namespace DrunkYard.DataAccess.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsLoggedIn { get; set; }
    }
}