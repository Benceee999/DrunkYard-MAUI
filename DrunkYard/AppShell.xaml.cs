namespace DrunkYard
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.GameDetailsPage), typeof(Views.GameDetailsPage));
            Routing.RegisterRoute(nameof(Views.SwipeGamePage), typeof(Views.SwipeGamePage));
            Routing.RegisterRoute(nameof(Views.KingsCupPage), typeof(Views.KingsCupPage));
        }
    }
}