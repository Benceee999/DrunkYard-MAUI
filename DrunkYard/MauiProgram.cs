using Microsoft.Extensions.Logging;
using DrunkYard.DataAccess.Services;

namespace DrunkYard
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });



            builder.Services.AddSingleton<DrunkYard.DataAccess.Services.LocalDbService>();
            builder.Services.AddSingleton<DrunkYard.DataAccess.Services.RestService>();

#if ANDROID
            builder.Services.AddSingleton<DrunkYard.Services.IToastService, DrunkYard.Platforms.Android.ToastService>();
#else
            builder.Services.AddSingleton<DrunkYard.Services.IToastService, DrunkYard.Services.ToastServiceStub>();
#endif

            // ViewModels
            builder.Services.AddTransient<DrunkYard.ViewModels.GamesViewModel>();
            builder.Services.AddTransient<DrunkYard.ViewModels.GameDetailsViewModel>();
            builder.Services.AddTransient<DrunkYard.ViewModels.SwipeGameViewModel>();
            builder.Services.AddTransient<DrunkYard.ViewModels.AddGameViewModel>();
            builder.Services.AddTransient<DrunkYard.ViewModels.VoteViewModel>();
            builder.Services.AddTransient<DrunkYard.ViewModels.KingsCupViewModel>();


            // Views
            builder.Services.AddTransient<DrunkYard.Views.GamesPage>();
            builder.Services.AddTransient<DrunkYard.Views.GameDetailsPage>();
            builder.Services.AddTransient<DrunkYard.Views.SwipeGamePage>();
            builder.Services.AddTransient<DrunkYard.Views.AddGamePage>();
            builder.Services.AddTransient<DrunkYard.Views.VotePage>();
            builder.Services.AddTransient<DrunkYard.Views.KingsCupPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
