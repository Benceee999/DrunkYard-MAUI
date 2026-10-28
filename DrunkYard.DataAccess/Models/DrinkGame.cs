using SQLite;
using System.Text.Json.Serialization;

namespace DrunkYard.DataAccess.Models
{
    public enum GameCategory { Easy, Medium, Hard, Extreme }

    public class DrinkGame
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Rules { get; set; }

        public GameCategory Category { get; set; }

        public int MinPlayers { get; set; }
        public bool IsEquipmentNeeded { get; set; }

        public bool IsFavorite { get; set; }

        public string RemoteId { get; set; }

        public string ImagePath { get; set; }

        public string PlayableLogicId { get; set; }
    }
}