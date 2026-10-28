using DrunkYard.ViewModels;

namespace DrunkYard.Views
{
    public partial class AddGamePage : ContentPage
    {
        public AddGamePage(AddGameViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}