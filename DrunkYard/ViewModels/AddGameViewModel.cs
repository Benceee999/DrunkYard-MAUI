using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;
using DrunkYard.Services;

namespace DrunkYard.ViewModels
{
    public partial class AddGameViewModel : ObservableObject
    {
        private readonly LocalDbService _dbService;
        private readonly RestService _restService;
        private readonly IToastService _toastService;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        string rules;

        [ObservableProperty]
        int minPlayers = 2;

        [ObservableProperty]
        bool isEquipmentNeeded;

        [ObservableProperty]
        int selectedCategoryIndex = 1;

        [ObservableProperty]
        string selectedImagePath;

        public AddGameViewModel(LocalDbService dbService, RestService restService, IToastService toastService)
        {
            _dbService = dbService;
            _restService = restService;
            _toastService = toastService;
        }

        [RelayCommand]
        public async Task TakePhoto()
        {
            try
            {
                string action = await Shell.Current.DisplayActionSheet("Kép hozzáadása", "Mégse", null, "Kamera", "Galéria");
                FileResult photo = null;

                if (action == "Kamera" && MediaPicker.Default.IsCaptureSupported)
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                else if (action == "Galéria")
                    photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(newFile);
                    await stream.CopyToAsync(newStream);
                    SelectedImagePath = newFile;
                }
            }
            catch { _toastService.Show("Hiba a kép kiválasztásakor."); }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description))
            {
                _toastService.Show("A Cím és a Leírás kötelező!");
                return;
            }

            var newCandidate = new CandidateGame
            {
                Id = Guid.NewGuid().ToString(),
                Title = this.Title,
                Description = this.Description,
                Rules = this.Rules ?? "",
                Category = (GameCategory)SelectedCategoryIndex,
                MinPlayers = this.MinPlayers,
                IsEquipmentNeeded = this.IsEquipmentNeeded,
                Votes = 0
            };

            await _dbService.SaveCandidateAsync(newCandidate);

            try
            {
                await _restService.SubmitCandidateGameAsync(newCandidate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API hiba, de helyben rögzítve: {ex.Message}");
            }

            _toastService.Show("Játék beküldve szavazásra!");

            Title = string.Empty;
            Description = string.Empty;
            Rules = string.Empty;
            SelectedImagePath = null;
            MinPlayers = 2;
            IsEquipmentNeeded = false;
        }
    }
}