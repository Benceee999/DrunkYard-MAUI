using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Text.Json.Serialization;

namespace DrunkYard.DataAccess.Models
{
    public partial class CandidateGame : ObservableObject
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Rules { get; set; } = string.Empty;

        public GameCategory Category { get; set; }

        public int MinPlayers { get; set; }
        public bool IsEquipmentNeeded { get; set; }

        [ObservableProperty]
        int votes;

        [property: SQLite.Ignore]
        [property: JsonIgnore]
        [ObservableProperty]
        bool hasVoted;
    }
}