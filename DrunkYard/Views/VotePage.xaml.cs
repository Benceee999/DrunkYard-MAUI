using DrunkYard.ViewModels;

namespace DrunkYard.Views
{
    public partial class VotePage : ContentPage
    {
        private readonly VoteViewModel _viewModel;

        public VotePage(VoteViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadCandidatesCommand.Execute(null);
        }
    }
}