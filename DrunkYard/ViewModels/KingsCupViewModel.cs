using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;

namespace DrunkYard.ViewModels
{
    public class PlayingCard
    {
        public string Suit { get; set; }
        public string Rank { get; set; }
        public string Color { get; set; }
        public string RuleTitle { get; set; }
        public string RuleDescription { get; set; }
    }

    public partial class KingsCupViewModel : ObservableObject
    {
        private List<PlayingCard> _deck = new();

        [ObservableProperty]
        PlayingCard currentCard;

        [ObservableProperty]
        int cardsRemaining;

        [ObservableProperty]
        string kingsCountText;

        private int _kingsDrawn = 0;

        public KingsCupViewModel()
        {
            StartNewGame();
        }

        [RelayCommand]
        public void StartNewGame()
        {
            GenerateDeck();
            ShuffleDeck();
            _kingsDrawn = 0;
            KingsCountText = "Királyok: 0 / 4";

            DrawCard();
        }

        [RelayCommand]
        public void DrawCard()
        {
            if (_deck.Count == 0)
            {
                StartNewGame();
                return;
            }

            var card = _deck[0];
            _deck.RemoveAt(0);

            if (card.Rank == "K")
            {
                _kingsDrawn++;
                KingsCountText = $"Királyok: {_kingsDrawn} / 4";

                if (_kingsDrawn == 4)
                {
                    card.RuleTitle = "KIRÁLYOK POHARA! 👑";
                    card.RuleDescription = "Ez volt az utolsó Király! Idd meg a középső pohár tartalmát! (Kattints az új játékhoz)";

                    _deck.Clear();
                }
            }

            CurrentCard = card;
            CardsRemaining = _deck.Count;
        }

        private void GenerateDeck()
        {
            _deck.Clear();
            string[] suits = { "♠️", "♣️", "♥️", "♦️" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    string color = (suit == "♥️" || suit == "♦️") ? "#D32F2F" : "#212121";
                    var (title, desc) = GetRule(rank);

                    _deck.Add(new PlayingCard
                    {
                        Suit = suit,
                        Rank = rank,
                        Color = color,
                        RuleTitle = title,
                        RuleDescription = desc
                    });
                }
            }
        }

        private void ShuffleDeck()
        {
            Random rng = new Random();
            int n = _deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = _deck[k];
                _deck[k] = _deck[n];
                _deck[n] = value;
            }
        }

        private (string, string) GetRule(string rank)
        {
            return rank switch
            {
                "2" => ("Te!", "Válassz valakit, akinek innia kell."),
                "3" => ("Én!", "Te iszol egyet."),
                "4" => ("Padló!", "Mindenki érintse meg a padlót. Az utolsó iszik."),
                "5" => ("Ivópárbaj", "Hívj ki valakit, aki veszít, annak egy másik játékos piáját is le kell húznia."),
                "6" => ("Az 50 milliós kérdés!", "Tegyél fel egy kérdést bárkinek, ha tudja a választ iszol, ha nem tudja ő iszik"),
                "7" => ("Ég a keze!", "Mindenki tegye a hüvelykujját az asztalra. Az utolsó iszik."),
                "8" => ("Ivó pajtás!", "Válassz egy párt magadnak. Ha te iszol, ő is iszik."),
                "9" => ("Rím!", "Mondj egy szót. Körbe kell rímelni rá. Aki elrontja, iszik."),
                "10" => ("Kategória!", "Válassz egy kategóriát. Soroljatok fel dolgokat. Aki ismétel/megakad, iszik."),
                "J" => ("Fiúk!", "Minden fiú iszik."),
                "Q" => ("Lányok!", "Minden lány iszik."),
                "K" => ("Önts bele!", "Önts egy kicsit az italodból a középső pohárba."),
                "A" => ("Huzi vagy ...", "Mindenki húzza le az italát, aki utoljára végez iszik egy rövidet."),
                _ => ("Igyál!", "Nincs szabály, csak igyál.")
            };
        }
    }
}