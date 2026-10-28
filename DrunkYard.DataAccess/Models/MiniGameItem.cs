using SQLite;

namespace DrunkYard.DataAccess.Models
{
    public enum MiniGameType
    {
        NeverHaveIEver,
        TruthOrDare,
        KingsCupCard
    }

    public class MiniGameItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Text { get; set; }
        public MiniGameType Type { get; set; }

        public string ImageUrl { get; set; }
    }
}