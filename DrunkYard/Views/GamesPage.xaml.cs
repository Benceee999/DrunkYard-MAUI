using DrunkYard.ViewModels;

namespace DrunkYard.Views
{
    public partial class GamesPage : ContentPage
    {
        private readonly GamesViewModel _viewModel;

        public GamesPage(GamesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadGamesCommand.ExecuteAsync(null);
        }
    }
}