using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;

namespace DrunkYard.ViewModels
{
    public partial class GamesViewModel : ObservableObject
    {
        private readonly LocalDbService _dbService;
        private readonly RestService _restService;

        public ObservableCollection<DrinkGame> Games { get; set; } = new();

        private List<DrinkGame> _allGames = new();

        [ObservableProperty]
        bool isBusy;

        [ObservableProperty]
        bool isFavoritesOnly;

        [ObservableProperty]
        int sortOptionIndex = 0;

        public GamesViewModel(LocalDbService dbService, RestService restService)
        {
            _dbService = dbService;
            _restService = restService;
            LoadGamesCommand.Execute(null);
        }

        partial void OnIsFavoritesOnlyChanged(bool value)
        {
            ApplyFilters();
        }

        partial void OnSortOptionIndexChanged(int value)
        {
            ApplyFilters();
        }

        [RelayCommand]
        public async Task LoadGames()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                _allGames = await _dbService.GetGamesAsync();

                var remoteGames = await _restService.RefreshDataAsync();
                if (remoteGames.Count > 0)
                {
                    await _dbService.SaveGamesFromApiAsync(remoteGames);
                    _allGames = await _dbService.GetGamesAsync();
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allGames.AsEnumerable();

            if (IsFavoritesOnly)
            {
                filtered = filtered.Where(g => g.IsFavorite);
            }

            switch (SortOptionIndex)
            {
                case 0:
                    filtered = filtered.OrderBy(g => g.Title);
                    break;
                case 1:
                    filtered = filtered.OrderBy(g => g.Category);
                    break;
                case 2:
                    filtered = filtered.OrderByDescending(g => g.Category);
                    break;
            }

            Games.Clear();
            foreach (var game in filtered)
            {
                Games.Add(game);
            }
        }

        [RelayCommand]
        public async Task Tap(DrinkGame game)
        {
            if (game == null) return;
            await Shell.Current.GoToAsync(nameof(Views.GameDetailsPage), new Dictionary<string, object>
            {
                { "Game", game }
            });
        }
    }
}