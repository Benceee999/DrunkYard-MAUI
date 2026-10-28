using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using DrunkYard.DataAccess.Models;
using DrunkYard.DataAccess.Services;

namespace DrunkYard.ViewModels
{
    public partial class SwipeGameViewModel : ObservableObject
    {
        private readonly LocalDbService _dbService;

        private List<MiniGameItem> _allQuestions = new();

        private int _currentIndex = 0;

        [ObservableProperty]
        MiniGameItem currentQuestion;

        [ObservableProperty]
        bool isGameOver;

        public SwipeGameViewModel(LocalDbService dbService)
        {
            _dbService = dbService;
            LoadQuestionsCommand.Execute(null);
        }

        [RelayCommand]
        public async Task LoadQuestions()
        {
            _allQuestions = await _dbService.GetMiniGameItemsAsync(MiniGameType.NeverHaveIEver);

            _allQuestions = _allQuestions.OrderBy(x => Guid.NewGuid()).ToList();

            _currentIndex = 0;
            IsGameOver = false;

            ShowNextCard();
        }

        public void ShowNextCard()
        {
            if (_currentIndex < _allQuestions.Count)
            {
                CurrentQuestion = _allQuestions[_currentIndex];
                _currentIndex++;
            }
            else
            {
                CurrentQuestion = null;
                IsGameOver = true;
            }
        }

        [RelayCommand]
        public void CardSwiped()
        {
            ShowNextCard();
        }
    }
}