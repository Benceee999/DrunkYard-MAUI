using DrunkYard.ViewModels;
namespace DrunkYard.Views
{
    public partial class GameDetailsPage : ContentPage
    {
        public GameDetailsPage(GameDetailsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}