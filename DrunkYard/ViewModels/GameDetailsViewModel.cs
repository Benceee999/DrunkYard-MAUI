using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;
using DrunkYard.Services;

namespace DrunkYard.ViewModels
{
    [QueryProperty(nameof(Game), "Game")]
    public partial class GameDetailsViewModel : ObservableObject
    {
        private readonly LocalDbService _dbService;
        private readonly IToastService _toastService;

        public GameDetailsViewModel(LocalDbService dbService, IToastService toastService)
        {
            _dbService = dbService;
            _toastService = toastService;
        }

        [ObservableProperty]
        DrinkGame game;

        [RelayCommand]
        public async Task ToggleFavorite()
        {
            if (Game == null) return;

            Game.IsFavorite = !Game.IsFavorite;

            OnPropertyChanged(nameof(Game));
            await _dbService.UpdateGameAsync(Game);

            if (Game.IsFavorite)
            {
                _toastService.Show("Hozzáadva a kedvencekhez!");
            }
            else
            {
                _toastService.Show("Eltávolítva a kedvencekből.");
            }
        }

        [RelayCommand]
        public async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task PlayGame()
        {
            if (Game == null || string.IsNullOrEmpty(Game.PlayableLogicId)) return;

            if (Game.PlayableLogicId == "swipe")
            {
                await Shell.Current.GoToAsync(nameof(Views.SwipeGamePage));
            }
            else if (Game.PlayableLogicId == "kingscup")
            {
                await Shell.Current.GoToAsync(nameof(Views.KingsCupPage));
            }
            else
            {
                await Shell.Current.DisplayAlert("Hiba", "Ehhez a játékhoz még nincs logika írva.", "OK");
            }
        }
    }
}