using DrunkYard.ViewModels;

namespace DrunkYard.Views
{
    public partial class KingsCupPage : ContentPage
    {
        public KingsCupPage(KingsCupViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
        }
    }
}