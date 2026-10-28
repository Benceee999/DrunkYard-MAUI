using DrunkYard.ViewModels;

namespace DrunkYard.Views
{
    public partial class SwipeGamePage : ContentPage
    {
        private readonly SwipeGameViewModel _viewModel;
        double _initialX;
        double _initialY;

        public SwipeGamePage(SwipeGameViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Accelerometer.Default.IsSupported)
            {
                Accelerometer.Default.ShakeDetected += Accelerometer_ShakeDetected;
            }
            ToggleAccelerometer(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (Accelerometer.Default.IsSupported)
            {
                Accelerometer.Default.ShakeDetected -= Accelerometer_ShakeDetected;
            }
            ToggleAccelerometer(false);
        }

        private void ToggleAccelerometer(bool start)
        {
            if (Accelerometer.Default.IsSupported)
            {
                if (start && !Accelerometer.Default.IsMonitoring)
                    Accelerometer.Default.Start(SensorSpeed.Game);
                else if (!start && Accelerometer.Default.IsMonitoring)
                    Accelerometer.Default.Stop();
            }
        }

        private void Accelerometer_ShakeDetected(object? sender, EventArgs e)
        {
            try
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }
            catch { }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                _viewModel.CardSwipedCommand.Execute(null);
            });
        }

        private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _initialX = SwipeCard.TranslationX;
                    _initialY = SwipeCard.TranslationY;
                    break;

                case GestureStatus.Running:
                    SwipeCard.TranslationX = _initialX + e.TotalX;
                    SwipeCard.Rotation = SwipeCard.TranslationX / 20;
                    break;

                case GestureStatus.Completed:
                    if (SwipeCard.TranslationX > 100)
                    {
                        try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); } catch { }

                        await SwipeCard.TranslateTo(500, 0, 200, Easing.Linear);
                        _viewModel.CardSwipedCommand.Execute(null);
                        ResetCard();
                    }
                    else if (SwipeCard.TranslationX < -100)
                    {
                        try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); } catch { }

                        await SwipeCard.TranslateTo(-500, 0, 200, Easing.Linear);
                        _viewModel.CardSwipedCommand.Execute(null);
                        ResetCard();
                    }
                    else
                    {
                        await SwipeCard.TranslateTo(0, 0, 200, Easing.SpringOut);
                        await SwipeCard.RotateTo(0, 200);
                    }
                    break;
            }
        }

        private void ResetCard()
        {
            SwipeCard.TranslationX = 0;
            SwipeCard.TranslationY = 0;
            SwipeCard.Rotation = 0;
        }
    }
}