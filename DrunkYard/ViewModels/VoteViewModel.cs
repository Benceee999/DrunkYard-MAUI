using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;
using DrunkYard.Services;

namespace DrunkYard.ViewModels
{
    public partial class VoteViewModel : ObservableObject
    {
        private readonly LocalDbService _dbService;
        private readonly RestService _restService;
        private readonly IToastService _toastService;

        public ObservableCollection<CandidateGame> Candidates { get; } = new();

        [ObservableProperty]
        bool isBusy;

        public VoteViewModel(LocalDbService dbService, RestService restService, IToastService toastService)
        {
            _dbService = dbService;
            _restService = restService;
            _toastService = toastService;
        }

        [RelayCommand]
        public async Task LoadCandidates()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var localCandidates = await _dbService.GetCandidatesAsync();
                DisplayCandidates(localCandidates);

                var remoteList = await _restService.GetCandidatesAsync();
                if (remoteList != null && remoteList.Count > 0)
                {
                    await _dbService.SaveCandidatesFromApiAsync(remoteList);
                    localCandidates = await _dbService.GetCandidatesAsync();
                    DisplayCandidates(localCandidates);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Szavazás betöltési hiba (helyi adatok maradnak): {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void DisplayCandidates(List<CandidateGame> list)
        {
            Candidates.Clear();
            foreach (var item in list)
            {
                item.HasVoted = Preferences.ContainsKey($"vote_{item.Id}");
                Candidates.Add(item);
            }
        }

        [RelayCommand]
        public async Task Vote(CandidateGame candidate)
        {
            if (candidate == null) return;

            if (candidate.HasVoted)
            {
                _toastService.Show("Erre már szavaztál!");
                return;
            }

            candidate.Votes++;
            candidate.HasVoted = true;

            Preferences.Set($"vote_{candidate.Id}", true);

            await _dbService.UpdateCandidateAsync(candidate);

            _toastService.Show($"Szavazat elküldve: {candidate.Title}");

            try
            {
                await _restService.VoteForCandidateAsync(candidate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API szinkron elmaradt, helyben mentve: {ex.Message}");
            }
        }
    }
}